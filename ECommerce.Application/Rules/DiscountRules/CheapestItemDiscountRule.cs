using ECommerce.Application.Abstractions.Discount;
using ECommerce.Application.DTOs.Discount;
using ECommerce.Domain.Entities;
using System.Linq;

namespace ECommerce.Application.Rules.DiscountRules
{
    public class CheapestItemDiscountRule : ICampaignRule
    {
        public string RuleType => "CheapestItemDiscount";

        public DiscountDetail? Calculate(CalculateDiscountRequest request, Campaign rule)
        {
            if (rule.MinQuantity == null || rule.DiscountRate == null) return null;

            var targetedItems = request.Items.AsEnumerable();

            if (!string.IsNullOrEmpty(rule.ProductId))
                targetedItems = targetedItems.Where(i => i.ProductId == rule.ProductId);
            else if (!string.IsNullOrEmpty(rule.CategoryId))
                targetedItems = targetedItems.Where(i => i.CategoryId == rule.CategoryId);
            else
                return null;

            var categoryItems = targetedItems.ToList();
            var totalQty = categoryItems.Sum(i => i.Quantity);

            if (totalQty < rule.MinQuantity.Value) return null;

            var cheapestItem = categoryItems.OrderBy(i => i.UnitPrice).FirstOrDefault();
            if (cheapestItem == null) return null;

            var discountRate = rule.DiscountRate.Value / 100m;
            var discountAmount = Math.Round(cheapestItem.UnitPrice * discountRate, 2);

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
