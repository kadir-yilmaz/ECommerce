using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.ViewModels.Products
{
    public class VM_Create_Product
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }
        public IFormFileCollection? Files { get; set; }
    }
}
