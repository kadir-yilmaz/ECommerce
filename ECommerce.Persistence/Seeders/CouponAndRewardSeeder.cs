using ECommerce.Domain.Entities;
using ECommerce.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Seeders
{
    public static class CouponAndRewardSeeder
    {
        public static async Task SeedAsync(ECommerceDbContext context)
        {
            // 1. Seed Coupons if empty
            if (!await context.DiscountCoupons.AnyAsync())
            {
                var coupons = new List<DiscountCoupon>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Code = "HOSGELDIN100",
                        DiscountType = "Amount",
                        DiscountValue = 100m,
                        MinCartAmount = 300m,
                        IsActive = true,
                        ExpirationDate = DateTime.UtcNow.AddYears(1),
                        UsageLimit = null,
                        Scope = "Public",
                        IsRewardCoupon = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Code = "FIRSAT20",
                        DiscountType = "Percentage",
                        DiscountValue = 20m, // %20 indirim
                        MaxDiscountAmount = 200m, // Max 200 TL indirim
                        MinCartAmount = 500m,
                        IsActive = true,
                        ExpirationDate = DateTime.UtcNow.AddYears(1),
                        UsageLimit = null,
                        Scope = "Public",
                        IsRewardCoupon = false
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Code = "KAZAN50",
                        DiscountType = "Amount",
                        DiscountValue = 50m,
                        MinCartAmount = 200m,
                        IsActive = true,
                        ExpirationDate = DateTime.UtcNow.AddMonths(6),
                        UsageLimit = 1000,
                        Scope = "Public",
                        IsRewardCoupon = false
                    }
                };

                await context.DiscountCoupons.AddRangeAsync(coupons);
                Console.WriteLine($"[Seed Data] Toplam {coupons.Count} adet indirim kuponu tohumlandı.");
            }

            // 2. Seed Reward Rules if empty
            if (!await context.RewardRules.AnyAsync())
            {
                var rules = new List<RewardRule>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "500 TL Alışverişe 100 TL Kupon",
                        PeriodInDays = 30,
                        MinTotalSpent = 500m,
                        RewardDiscountType = "Amount",
                        RewardDiscountValue = 100m,
                        CouponValidityDays = 15,
                        CouponMinCartAmount = 300m,
                        IsActive = true
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "1500 TL Alışverişe 300 TL Kupon",
                        PeriodInDays = 30,
                        MinTotalSpent = 1500m,
                        RewardDiscountType = "Amount",
                        RewardDiscountValue = 300m,
                        CouponValidityDays = 30,
                        CouponMinCartAmount = 1000m,
                        IsActive = true
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "3000 TL Alışverişe %20 İndirim Kuponu",
                        PeriodInDays = 45,
                        MinTotalSpent = 3000m,
                        RewardDiscountType = "Percentage",
                        RewardDiscountValue = 20m,
                        RewardMaxDiscountAmount = 600m,
                        CouponValidityDays = 30,
                        CouponMinCartAmount = 1500m,
                        IsActive = true
                    }
                };

                await context.RewardRules.AddRangeAsync(rules);
                Console.WriteLine($"[Seed Data] Toplam {rules.Count} adet ödül kuralı (RewardRule) tohumlandı.");
            }

            await context.SaveChangesAsync();
        }
    }
}
