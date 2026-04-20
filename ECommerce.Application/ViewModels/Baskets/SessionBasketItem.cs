using System;

namespace ECommerce.Application.ViewModels.Baskets
{
    public class SessionBasketItem
    {
        public string BasketItemId { get; set; } = Guid.NewGuid().ToString();
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
