using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class ProductImageFile : File
    {
        public ProductImageFile()
        {
            Products = new HashSet<Product>();
        }
        public bool Showcase { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
