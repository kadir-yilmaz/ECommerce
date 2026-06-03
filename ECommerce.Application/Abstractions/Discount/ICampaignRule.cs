using ECommerce.Application.DTOs.Discount;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstractions.Discount
{
    public interface ICampaignRule
    {
        string RuleType { get; }
        
        /// <summary>
        /// Orijinal sepet (request) baz alınarak belirtilen kuralı (rule) test eder
        /// ve uyan kısımların sağladığı indirim tutarını hesaplar.
        /// </summary>
        DiscountDetail? Calculate(CalculateDiscountRequest request, Campaign rule);
    }
}
