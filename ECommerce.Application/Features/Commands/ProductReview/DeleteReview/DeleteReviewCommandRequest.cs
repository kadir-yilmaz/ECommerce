using MediatR;
using System;

namespace ECommerce.Application.Features.Commands.ProductReview.DeleteReview
{
    public class DeleteReviewCommandRequest : IRequest<DeleteReviewCommandResponse>
    {
        public Guid ReviewId { get; set; }
    }
}
