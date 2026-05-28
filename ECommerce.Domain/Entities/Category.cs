using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities
{
    public class Category : BaseEntity
    {
        public Category()
        {
            SubCategories = new HashSet<Category>();
            Products = new HashSet<Product>();
        }

        public string Name { get; set; }
        
        public Guid? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        
        public ICollection<Category> SubCategories { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
