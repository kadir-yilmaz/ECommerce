namespace ECommerce.Application.Features.Queries.CampaignImageFile.GetCampaignImages
{
    public class GetCampaignImagesQueryResponse
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Storage { get; set; }
        public bool Showcase { get; set; }
        public string? Title { get; set; }
    }
}
