using MediatR;
using System;

namespace ECommerce.Application.Features.Commands.ProductReview.ToggleReviewReaction
{
    public class ToggleReviewReactionCommandRequest : IRequest<ToggleReviewReactionCommandResponse>
    {
        public Guid ReviewId { get; set; }
        public bool IsLike { get; set; }
    }
}
