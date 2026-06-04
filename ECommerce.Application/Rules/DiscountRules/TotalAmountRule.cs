using ECommerce.Application.Abstractions.Discount;
using ECommerce.Application.DTOs.Discount;
using ECommerce.Domain.Entities;
using System.Linq;

namespace ECommerce.Application.Rules.DiscountRules
{
    public class TotalAmountRule : ICampaignRule
    {
        public string RuleType => "TotalAmountDiscount";

        public DiscountDetail? Calculate(CalculateDiscountRequest request, Campaign rule)
        {
            if (rule.MinAmount == null || rule.DiscountRate == null) return null;

            var targetedItems = request.Items.AsEnumerable();

            if (!string.IsNullOrEmpty(rule.ProductId))
                targetedItems = targetedItems.Where(i => string.Equals(i.ProductId, rule.ProductId, StringComparison.OrdinalIgnoreCase));
            else if (!string.IsNullOrEmpty(rule.CategoryId))
                targetedItems = targetedItems.Where(i => string.Equals(i.CategoryId, rule.CategoryId, StringComparison.OrdinalIgnoreCase));

            var categoryItems = targetedItems.ToList();
            var targetedTotal = categoryItems.Sum(i => i.UnitPrice * i.Quantity);
            
            // Sepet hedeflenen tutarı aştı mı?
            if (targetedTotal < rule.MinAmount.Value)
                return null;

            // Indirim Orjinal tutar uzerinden bagimsiz hesaplanir.
            var discountRate = rule.DiscountRate.Value / 100m;
            // Eger kategori/urun secilmis ise sadece o urunlere indirim yap. Yoksa tum sepete yap.
            var baseAmountForDiscount = categoryItems.Sum(i => i.UnitPrice * i.Quantity);
            var discountAmount = Math.Round(baseAmountForDiscount * discountRate, 2);

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
