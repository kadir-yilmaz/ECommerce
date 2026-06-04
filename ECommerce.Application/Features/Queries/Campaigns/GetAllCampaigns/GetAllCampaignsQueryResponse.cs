using System.Collections.Generic;

namespace ECommerce.Application.Features.Queries.Campaigns.GetAllCampaigns
{
    public class GetAllCampaignsQueryResponse
    {
        public List<CampaignDto> Campaigns { get; set; }
    }

    public class CampaignDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RuleType { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? DiscountRate { get; set; }
        public int? MinQuantity { get; set; }
        public int? FreeQuantity { get; set; }
        public string? ProductId { get; set; }
        public string? CategoryId { get; set; }
        public string? Brand { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
