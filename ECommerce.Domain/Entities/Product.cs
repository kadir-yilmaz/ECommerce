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
        }
        
        public string Name { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }

        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }

        //public ICollection<Order> Orders { get; set; }
        public ICollection<ProductImageFile> ProductImageFiles { get; set; }
        public ICollection<BasketItem> BasketItems { get; set; }
        public ICollection<FavoriteItem> FavoriteItems { get; set; }
    }
}
