using ECommerce.Application.Abstractions.Discount;
using ECommerce.Application.DTOs.Discount;
using ECommerce.Domain.Entities;
using System.Linq;

namespace ECommerce.Application.Rules.DiscountRules
{
    public class FreeShippingRule : ICampaignRule
    {
        public string RuleType => "FreeShipping";

        public DiscountDetail? Calculate(CalculateDiscountRequest request, Campaign rule)
        {
            if (rule.MinAmount == null) return null;

            var originalTotal = request.Items.Sum(i => i.UnitPrice * i.Quantity);

            if (originalTotal >= rule.MinAmount.Value)
            {
                return new DiscountDetail
                {
                    DiscountName = rule.Name,
                    DiscountType = "FreeShipping",
                    DiscountAmount = 0 // Kargo ucreti sepet icerisinde bir urun degilse amount 0 donulup IsShippingFree true set edilir
                };
            }

            return null;
        }
    }
}
