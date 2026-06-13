namespace ECommerce.Application.Features.Queries.Product.GetByIdProduct
{
    public class GetByIdProductQueryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public float Price { get; set; }
        public bool ShowOnHomepage { get; set; }
        public System.Guid? CategoryId { get; set; }
        public string Brand { get; set; }
        public float AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public System.Collections.Generic.List<ECommerce.Application.DTOs.Product.ProductCampaignDto> Campaigns { get; set; }
        public string? ImagePath { get; set; }
        public System.Collections.Generic.List<GetByIdProductImageResponse> ProductImageFiles { get; set; }
    }

    public class GetByIdProductImageResponse
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public bool Showcase { get; set; }
    }
}
