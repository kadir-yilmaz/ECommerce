using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        public Product()
        {
            ProductImageFiles = new HashSet<ProductImageFile>();
            BasketItems = new HashSet<BasketItem>();
            FavoriteItems = new HashSet<FavoriteItem>();
            Reviews = new HashSet<ProductReview>();
        }
        public string Brand { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }
        public bool ShowOnHomepage { get; set; }
        public bool Showcase { get; set; }

        /// <summary>
        /// Onaylı yorumların ortalama puanı
        /// </summary>
        public float AverageRating { get; set; }

        /// <summary>
        /// Onaylı yorum sayısı
        /// </summary>
        public int ReviewCount { get; set; }

        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }

        //public ICollection<Order> Orders { get; set; }
        public ICollection<ProductImageFile> ProductImageFiles { get; set; }
        public ICollection<BasketItem> BasketItems { get; set; }
        public ICollection<FavoriteItem> FavoriteItems { get; set; }
        public ICollection<ProductReview> Reviews { get; set; }
    }
}
