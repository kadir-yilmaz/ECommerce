namespace ECommerce.Application.Features.Commands.ProductReview.ToggleReviewReaction
{
    public class ToggleReviewReactionCommandResponse
    {
        public bool Success { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public string? CurrentUserReaction { get; set; }
        public string Message { get; set; }
    }
}
