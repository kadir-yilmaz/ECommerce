using ECommerce.Application.Abstractions.Discount;
using ECommerce.Application.DTOs.Discount;
using ECommerce.Domain.Entities;
using System;
using System.Linq;

namespace ECommerce.Application.Rules.DiscountRules
{
    public class SelectedProductsDiscountRule : ICampaignRule
    {
        public string RuleType => "SelectedProductsDiscount";

        public DiscountDetail? Calculate(CalculateDiscountRequest request, Campaign rule)
        {
            if (string.IsNullOrEmpty(rule.ProductId) || rule.DiscountRate == null) return null;

            var productIds = rule.ProductId.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!productIds.Any()) return null;

            var targetedItems = request.Items.Where(i => productIds.Contains(i.ProductId, StringComparer.OrdinalIgnoreCase)).ToList();
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
