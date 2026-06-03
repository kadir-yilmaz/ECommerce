namespace ECommerce.Application.DTOs.Product
{
    public class ProductCampaignDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RuleType { get; set; }
        public decimal? DiscountRate { get; set; }
    }
}
