namespace ECommerce.Application.Features.Queries.ProductReview.CanUserReview
{
    public class CanUserReviewQueryResponse
    {
        public bool CanReview { get; set; }
        public string? Reason { get; set; }
    }
}
