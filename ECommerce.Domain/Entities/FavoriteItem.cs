using System;

namespace ECommerce.Domain.Entities
{
    public class FavoriteItem : BaseEntity
    {
        public Guid FavoriteId { get; set; }
        public Guid ProductId { get; set; }

        public Favorite Favorite { get; set; }
        public Product Product { get; set; }
    }
}
