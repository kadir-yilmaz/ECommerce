using System;

namespace ECommerce.Domain.Entities
{
    public class ProductReview : BaseEntity
    {
        public Guid ProductId { get; set; }
        public string UserId { get; set; }

        /// <summary>
        /// 1-5 arası puan
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Yorum metni
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 0=Pending, 1=Approved, 2=Rejected
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Soft delete bayrağı
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Silinme tarihi
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Admin onay/red notu
        /// </summary>
        public string? AdminNote { get; set; }

        /// <summary>
        /// Küfür tespit edildi mi
        /// </summary>
        public bool HasProfanity { get; set; }

        /// <summary>
        /// Fiyat bilgisi tespit edildi mi
        /// </summary>
        public bool HasPriceInfo { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public AppUser User { get; set; }
    }
}
