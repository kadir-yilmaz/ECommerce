using ECommerce.Application.Abstractions.Discount;
using ECommerce.Application.DTOs.Discount;
using ECommerce.Application.Repositories.Campaign;
using ECommerce.Application.Repositories.DiscountCoupon;
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

        public DiscountService(IEnumerable<ICampaignRule> campaignRules, ICampaignReadRepository campaignReadRepository, IDiscountCouponReadRepository discountCouponReadRepository)
        {
            _campaignRules = campaignRules;
            _campaignReadRepository = campaignReadRepository;
            _discountCouponReadRepository = discountCouponReadRepository;
        }

        public async Task<CalculateDiscountResponse> CalculateDiscountAsync(CalculateDiscountRequest request)
        {
            var response = new CalculateDiscountResponse();
            response.OriginalTotal = request.Items.Sum(i => i.UnitPrice * i.Quantity);
            response.FinalTotal = response.OriginalTotal;

            // 1. Kampanyalari kontrol et ve hesapla (Her biri Orjinal fiyat ustunden hesaplanir)
            var activeCampaigns = _campaignReadRepository.GetWhere(c => c.IsActive).ToList();
            
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
                if (coupon != null && (coupon.ExpirationDate == null || coupon.ExpirationDate > DateTime.UtcNow) && (coupon.UsageLimit == null || coupon.UsedCount < coupon.UsageLimit))
                {
                    if (response.OriginalTotal >= coupon.MinCartAmount)
                    {
                        decimal couponDiscountAmount = 0;
                        if (coupon.DiscountType == "Amount")
                        {
                            couponDiscountAmount = coupon.DiscountValue;
                        }
                        else if (coupon.DiscountType == "Percentage")
                        {
                            couponDiscountAmount = Math.Round(response.OriginalTotal * (coupon.DiscountValue / 100m), 2);
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

            // 3. Toplamlari guncelle
            response.TotalDiscount = response.AppliedDiscounts.Sum(d => d.DiscountAmount);
            response.FinalTotal = response.OriginalTotal - response.TotalDiscount;

            // Fiyat negatif olamaz
            if (response.FinalTotal < 0)
                response.FinalTotal = 0;

            // FreeShipping bayragini kontrol et
            if (response.AppliedDiscounts.Any(d => d.DiscountType == "FreeShipping"))
            {
                response.IsShippingFree = true;
            }

            return response;
        }
    }
}
