using System.Collections.Generic;

namespace ECommerce.Application.ViewModels.Baskets
{
    public class SessionBasket
    {
        public string BasketId { get; set; }
        public List<SessionBasketItem> BasketItems { get; set; } = new();
    }
}
