using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class DiscountCoupon : BaseEntity
    {
        public string Code { get; set; }

        /// <summary>
        /// İndirim Tipi: Amount (Örn: 50 TL) veya Percentage (Örn: %15)
        /// </summary>
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// Kuponun kullanılması için sepetin asıl tutarının minimum limiti (Örn: 400 TL)
        /// </summary>
        public decimal MinCartAmount { get; set; }

        public bool IsActive { get; set; }
        public DateTime? ExpirationDate { get; set; }
        
        /// <summary>
        /// Kaç kişinin kullanabileceği (null ise sınırsız)
        /// </summary>
        public int? UsageLimit { get; set; }

        /// <summary>
        /// Şu ana kadar kaç kez kullanıldı
        /// </summary>
        public int UsedCount { get; set; }
    }
}
