namespace ECommerce.Application.Features.Commands.ProductReview.CreateReview
{
    public class CreateReviewCommandResponse
    {
        public string Id { get; set; }
        public bool HasProfanity { get; set; }
        public bool HasPriceInfo { get; set; }
    }
}
