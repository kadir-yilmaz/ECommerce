using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class OrderDiscount : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        /// <summary>
        /// Uygulanan kampanyanın veya kuponun adı
        /// </summary>
        public string DiscountName { get; set; }

        /// <summary>
        /// Tip: "Campaign", "Coupon" veya "FreeShipping" vb.
        /// </summary>
        public string DiscountType { get; set; }

        /// <summary>
        /// Asıl fiyat üzerinden hesaplanarak sağlanan net indirim tutarı
        /// </summary>
        public decimal DiscountAmount { get; set; }
    }
}
