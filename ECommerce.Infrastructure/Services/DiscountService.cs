using ECommerce.Application.Abstractions.Discount;
using ECommerce.Application.DTOs.Discount;
using ECommerce.Application.Repositories.Campaign;
using ECommerce.Application.Repositories.DiscountCoupon;
using ECommerce.Application.Repositories.UserCoupon;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IEnumerable<ICampaignRule> _campaignRules;
        private readonly ICampaignReadRepository _campaignReadRepository;
        private readonly IDiscountCouponReadRepository _discountCouponReadRepository;
        private readonly IDiscountCouponWriteRepository _discountCouponWriteRepository;
        private readonly IUserCouponReadRepository _userCouponReadRepository;
        private readonly IUserCouponWriteRepository _userCouponWriteRepository;

        public DiscountService(
            IEnumerable<ICampaignRule> campaignRules,
            ICampaignReadRepository campaignReadRepository,
            IDiscountCouponReadRepository discountCouponReadRepository,
            IDiscountCouponWriteRepository discountCouponWriteRepository,
            IUserCouponReadRepository userCouponReadRepository,
            IUserCouponWriteRepository userCouponWriteRepository)
        {
            _campaignRules = campaignRules;
            _campaignReadRepository = campaignReadRepository;
            _discountCouponReadRepository = discountCouponReadRepository;
            _discountCouponWriteRepository = discountCouponWriteRepository;
            _userCouponReadRepository = userCouponReadRepository;
            _userCouponWriteRepository = userCouponWriteRepository;
        }

        public async Task<CalculateDiscountResponse> CalculateDiscountAsync(CalculateDiscountRequest request)
        {
            var response = new CalculateDiscountResponse();
            response.OriginalTotal = request.Items.Sum(i => i.UnitPrice * i.Quantity);
            response.FinalTotal = response.OriginalTotal;

            // 1. Kampanyalari kontrol et ve hesapla (Her biri Orjinal fiyat ustunden hesaplanir)
            var activeCampaigns = _campaignReadRepository.GetWhere(c => c.IsActive && (c.EndDate == null || c.EndDate > DateTime.UtcNow)).ToList();
            
            foreach (var campaign in activeCampaigns)
            {
                var ruleEngine = _campaignRules.FirstOrDefault(r => r.RuleType == campaign.RuleType);
                if (ruleEngine != null)
                {
                    var discountDetail = ruleEngine.Calculate(request, campaign);
                    if (discountDetail != null)
                    {
                        response.AppliedDiscounts.Add(discountDetail);
                    }
                }
            }

            // 2. Kupon kodunu kontrol et
            if (!string.IsNullOrEmpty(request.CouponCode))
            {
                var coupon = _discountCouponReadRepository.GetWhere(c => c.Code == request.CouponCode && c.IsActive).FirstOrDefault();
                if (coupon != null && (coupon.ExpirationDate == null || coupon.ExpirationDate > DateTime.UtcNow))
                {
                    bool canUseCoupon = false;

                    if (!string.IsNullOrEmpty(request.UserId))
                    {
                        var userCoupon = await _userCouponReadRepository.GetWhere(
                            uc => uc.DiscountCouponId == coupon.Id && uc.UserId == request.UserId, false
                        ).FirstOrDefaultAsync();

                        if (coupon.Scope == "Private")
                        {
                            // Private ise atanmış olmalı ve kullanılmamış olmalı
                            canUseCoupon = userCoupon != null && !userCoupon.IsUsed;
                        }
                        else 
                        {
                            // Public ise ya kayıt yok (daha önce kullanmamış) ya da IsUsed == false (böyle bir senaryo public'te pek olmaz ama garantilemek için)
                            canUseCoupon = userCoupon == null || !userCoupon.IsUsed;
                        }
                    }
                    else
                    {
                        // Kullanıcı giriş yapmamışsa sepette kupon indirimini "geçici" olarak gösterebilmek için Public ise izin ver
                        // Gerçek sipariş verilirken zaten giriş yapması istenecek ve o zaman kontrol edilecek.
                        if (coupon.Scope == "Public")
                        {
                            canUseCoupon = true;
                        }
                    }

                    if (canUseCoupon && response.OriginalTotal >= coupon.MinCartAmount)
                    {
                        if (coupon.DiscountType == "FreeShipping")
                        {
                            // FreeShipping kupon: doğrudan kargo ücretsiz
                            response.AppliedDiscounts.Add(new DiscountDetail
                            {
                                DiscountName = coupon.Code,
                                DiscountType = "FreeShipping",
                                DiscountAmount = 0
                            });
                        }
                        else
                        {
                            decimal couponDiscountAmount = 0;
                            if (coupon.DiscountType == "Amount")
                            {
                                couponDiscountAmount = coupon.DiscountValue;
                            }
                            else if (coupon.DiscountType == "Percentage")
                            {
                                couponDiscountAmount = Math.Round(response.OriginalTotal * (coupon.DiscountValue / 100m), 2);
                                // Max indirim limiti kontrolü
                                if (coupon.MaxDiscountAmount.HasValue && couponDiscountAmount > coupon.MaxDiscountAmount.Value)
                                    couponDiscountAmount = coupon.MaxDiscountAmount.Value;
                            }

                            if (couponDiscountAmount > 0)
                            {
                                response.AppliedDiscounts.Add(new DiscountDetail
                                {
                                    DiscountName = coupon.Code,
                                    DiscountType = "Coupon",
                                    DiscountAmount = couponDiscountAmount
                                });
                            }
                        }
                    }
                }
            }

            // 3. Toplamlari guncelle
            response.TotalDiscount = response.AppliedDiscounts.Sum(d => d.DiscountAmount);
            response.FinalTotal = response.OriginalTotal - response.TotalDiscount;

            // Fiyat negatif olamaz
            if (response.FinalTotal < 0)
                response.FinalTotal = 0;

            // FreeShipping bayragini kontrol et ve kargo ayarlarini dinamik yukle
            var shippingCampaign = _campaignReadRepository.GetWhere(c => c.RuleType == "FreeShipping" && (c.EndDate == null || c.EndDate > DateTime.UtcNow)).FirstOrDefault();
            decimal defaultShippingFee = shippingCampaign?.DiscountRate ?? 50m;
            decimal shippingThreshold = (shippingCampaign != null && shippingCampaign.IsActive) ? (shippingCampaign.MinAmount ?? 500m) : 0m;

            response.IsShippingFree = response.AppliedDiscounts.Any(d => d.DiscountType == "FreeShipping");
            response.ShippingFee = response.IsShippingFree ? 0m : defaultShippingFee;
            response.ShippingThreshold = shippingThreshold;

            return response;
        }
    }
}

