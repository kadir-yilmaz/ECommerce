using MediatR;
using System;

namespace ECommerce.Application.Features.Commands.ProductReview.UpdateReview
{
    public class UpdateReviewCommandRequest : IRequest<UpdateReviewCommandResponse>
    {
        public Guid ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
