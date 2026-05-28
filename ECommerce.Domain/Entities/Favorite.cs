using System.Collections.Generic;

namespace ECommerce.Domain.Entities
{
    public class Favorite : BaseEntity
    {
        public Favorite()
        {
            FavoriteItems = new HashSet<FavoriteItem>();
        }

        public string UserId { get; set; }

        public AppUser User { get; set; }
        public ICollection<FavoriteItem> FavoriteItems { get; set; }
    }
}
