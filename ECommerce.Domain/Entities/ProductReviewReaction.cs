using System;

namespace ECommerce.Domain.Entities
{
    public class ProductReviewReaction : BaseEntity
    {
        public Guid ReviewId { get; set; }
        public string UserId { get; set; }
        public bool IsLike { get; set; }

        // Navigation properties
        public ProductReview Review { get; set; }
        public AppUser User { get; set; }
    }
}
