using System;

namespace ECommerce.Application.Features.Queries.ProductReview.GetUnreviewedProducts
{
    public class GetUnreviewedProductsQueryResponse
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductBrand { get; set; }
        public string ProductImagePath { get; set; }
        public float ProductPrice { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
