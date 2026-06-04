using ECommerce.Application.Abstractions.Discount;
using ECommerce.Application.Repositories;
using ECommerce.Application.Repositories.DiscountCoupon;
using ECommerce.Application.Repositories.RewardRule;
using ECommerce.Application.Repositories.UserCoupon;
using ECommerce.Application.Abstractions.Hubs;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services
{
    public class RewardService : IRewardService
    {
        private readonly IRewardRuleReadRepository _rewardRuleReadRepository;
        private readonly IDiscountCouponWriteRepository _discountCouponWriteRepository;
        private readonly IDiscountCouponReadRepository _discountCouponReadRepository;
        private readonly IUserCouponWriteRepository _userCouponWriteRepository;
        private readonly IUserCouponReadRepository _userCouponReadRepository;
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly ICouponHubService _couponHubService;
        private readonly ILogger<RewardService> _logger;

        public RewardService(
            IRewardRuleReadRepository rewardRuleReadRepository,
            IDiscountCouponWriteRepository discountCouponWriteRepository,
            IDiscountCouponReadRepository discountCouponReadRepository,
            IUserCouponWriteRepository userCouponWriteRepository,
            IUserCouponReadRepository userCouponReadRepository,
            IOrderReadRepository orderReadRepository,
            ICouponHubService couponHubService,
            ILogger<RewardService> logger)
        {
            _rewardRuleReadRepository = rewardRuleReadRepository;
            _discountCouponWriteRepository = discountCouponWriteRepository;
            _discountCouponReadRepository = discountCouponReadRepository;
            _userCouponWriteRepository = userCouponWriteRepository;
            _userCouponReadRepository = userCouponReadRepository;
            _orderReadRepository = orderReadRepository;
            _couponHubService = couponHubService;
            _logger = logger;
        }

        public async Task CheckAndGrantRewardsAsync(string userId)
        {
            var activeRules = _rewardRuleReadRepository.GetWhere(r => r.IsActive).ToList();
            if (!activeRules.Any())
                return;

            foreach (var rule in activeRules)
            {
                try
                {
                    var periodStart = DateTime.UtcNow.AddDays(-rule.PeriodInDays);

                    // Kullanıcının belirtilen süre içindeki teslim edilen siparişlerinin toplam tutarını hesapla
                    var totalSpent = await _orderReadRepository.Table
                        .Include(o => o.Basket)
                            .ThenInclude(b => b.BasketItems)
                                .ThenInclude(bi => bi.Product)
                        .Where(o => o.Basket.UserId == userId
                                    && o.Status >= 1 // Paid or above
                                    && o.CreatedDate >= periodStart)
                        .SelectMany(o => o.Basket.BasketItems)
                        .SumAsync(bi => (decimal)(bi.Product.Price * bi.Quantity));

                    if (totalSpent < rule.MinTotalSpent)
                        continue;

                    // Bu kural için daha önce kupon verilmiş mi kontrol et (aynı periyotta)
                    var alreadyRewarded = await _userCouponReadRepository.GetWhere(
                        uc => uc.UserId == userId && uc.DiscountCoupon.IsRewardCoupon, false
                    ).Include(uc => uc.DiscountCoupon)
                    .AnyAsync(uc => uc.CreatedDate >= periodStart
                                    && uc.DiscountCoupon.DiscountValue == rule.RewardDiscountValue
                                    && uc.DiscountCoupon.DiscountType == rule.RewardDiscountType);

                    if (alreadyRewarded)
                        continue;

                    // Yeni kupon oluştur
                    var couponCode = $"REWARD-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
                    var coupon = new DiscountCoupon
                    {
                        Code = couponCode,
                        DiscountType = rule.RewardDiscountType,
                        DiscountValue = rule.RewardDiscountValue,
                        MaxDiscountAmount = rule.RewardMaxDiscountAmount,
                        MinCartAmount = rule.CouponMinCartAmount,
                        IsActive = true,
                        ExpirationDate = DateTime.UtcNow.AddDays(rule.CouponValidityDays),
                        UsedCount = 0,
                        Scope = "Private",
                        IsRewardCoupon = true
                    };

                    await _discountCouponWriteRepository.AddAsync(coupon);
                    await _discountCouponWriteRepository.SaveAsync();

                    // Kullanıcıya ata
                    await _userCouponWriteRepository.AddAsync(new UserCoupon
                    {
                        DiscountCouponId = coupon.Id,
                        UserId = userId,
                        IsUsed = false
                    });
                    await _userCouponWriteRepository.SaveAsync();

                    _logger.LogInformation(
                        "Reward coupon {CouponCode} granted to user {UserId} for rule '{RuleName}'. Total spent: {TotalSpent}",
                        couponCode, userId, rule.Name, totalSpent);

                    string rewardText = rule.RewardDiscountType == "Percentage" 
                        ? $"%{(int)rule.RewardDiscountValue} İndirim" 
                        : rule.RewardDiscountType == "FreeShipping" 
                            ? "Kargo Bedava" 
                            : $"{rule.RewardDiscountValue} TL İndirim";

                    await _couponHubService.CouponAddedMessageAsync($"Tebrikler! {rule.Name} kuralı ile {rewardText} ödülü kazandınız. (Kupon Kodunuz: {couponCode})", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking reward rule '{RuleName}' for user {UserId}", rule.Name, userId);
                }
            }
        }
    }
}
