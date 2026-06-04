using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Basket.GetBasketItems
{
    public class GetBasketItemsQueryResponse
    {
        public string BasketItemId { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        public string? ImagePath { get; set; }
        public string Brand { get; set; }
        public List<ECommerce.Application.DTOs.Product.ProductCampaignDto> Campaigns { get; set; }
    }
}
