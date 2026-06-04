using ECommerce.Application.Abstractions.Discount;
using ECommerce.Application.DTOs.Discount;
using ECommerce.Domain.Entities;
using System.Linq;

namespace ECommerce.Application.Rules.DiscountRules
{
    public class BuyXGetYFreeRule : ICampaignRule
    {
        public string RuleType => "FreeItem";

        public DiscountDetail? Calculate(CalculateDiscountRequest request, Campaign rule)
        {
            if (rule.MinQuantity == null || rule.FreeQuantity == null) return null;

            var targetedItems = request.Items.AsEnumerable();

            if (!string.IsNullOrEmpty(rule.ProductId))
                targetedItems = targetedItems.Where(i => string.Equals(i.ProductId, rule.ProductId, StringComparison.OrdinalIgnoreCase));
            else if (!string.IsNullOrEmpty(rule.CategoryId))
                targetedItems = targetedItems.Where(i => string.Equals(i.CategoryId, rule.CategoryId, StringComparison.OrdinalIgnoreCase));
            else
                return null; // Bir urun veya kategori hedeflenmek zorundadir

            var categoryItems = targetedItems.ToList();
            var totalQty = categoryItems.Sum(i => i.Quantity);

            if (totalQty < rule.MinQuantity.Value) return null;

            var sets = totalQty / rule.MinQuantity.Value;
            var freeCount = sets * rule.FreeQuantity.Value;

            var unitList = categoryItems
                .SelectMany(i => Enumerable.Repeat(i.UnitPrice, i.Quantity))
                .OrderBy(price => price) // En ucuzdan sirala
                .ToList();

            var freeItems = unitList.Take((int)freeCount).ToList();
            var discountAmount = freeItems.Sum();

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
