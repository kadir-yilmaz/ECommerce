using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class Campaign : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        /// <summary>
        /// Kural Tipi: TotalAmount, BuyXGetYFree, CheapestItemDiscount, FreeShipping vb.
        /// </summary>
        public string RuleType { get; set; }
        public bool IsActive { get; set; }

        // Kural Parametreleri
        public decimal? MinAmount { get; set; }
        public decimal? DiscountRate { get; set; }
        public int? MinQuantity { get; set; }
        public int? FreeQuantity { get; set; }
        public string? ProductId { get; set; }
        public string? CategoryId { get; set; }
        public string? Brand { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
