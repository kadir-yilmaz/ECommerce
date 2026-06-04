using System;

namespace ECommerce.Domain.Entities
{
    public class UserCoupon : BaseEntity
    {
        public Guid DiscountCouponId { get; set; }
        public DiscountCoupon DiscountCoupon { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        /// <summary>
        /// Kullanıcı bu kuponu kullandı mı?
        /// </summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// Kullanım tarihi
        /// </summary>
        public DateTime? UsedDate { get; set; }
    }
}
