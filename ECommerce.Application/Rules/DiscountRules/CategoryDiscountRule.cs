using ECommerce.Application.Abstractions.Discount;
using ECommerce.Application.DTOs.Discount;
using ECommerce.Domain.Entities;
using System;
using System.Linq;

namespace ECommerce.Application.Rules.DiscountRules
{
    public class CategoryDiscountRule : ICampaignRule
    {
        public string RuleType => "CategoryDiscount";

        public DiscountDetail? Calculate(CalculateDiscountRequest request, Campaign rule)
        {
            if (string.IsNullOrEmpty(rule.CategoryId) || rule.DiscountRate == null) return null;

            var query = request.Items.AsEnumerable();
            query = query.Where(i => string.Equals(i.CategoryId, rule.CategoryId, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(rule.Brand))
            {
                query = query.Where(i => string.Equals(i.Brand, rule.Brand, StringComparison.OrdinalIgnoreCase));
            }

            var targetedItems = query.ToList();
            if (!targetedItems.Any()) return null;

            var targetedTotal = targetedItems.Sum(i => i.UnitPrice * i.Quantity);
            var discountRate = rule.DiscountRate.Value / 100m;
            var discountAmount = Math.Round(targetedTotal * discountRate, 2);

            if (discountAmount <= 0) return null;

            return new DiscountDetail
            {
                DiscountName = rule.Name,
                DiscountType = "Campaign",
                DiscountAmount = discountAmount
            };
        }
    }
}
