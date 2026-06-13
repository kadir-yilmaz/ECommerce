using MediatR;
using System;

namespace ECommerce.Application.Features.Commands.ProductReview.CreateReview
{
    public class CreateReviewCommandRequest : IRequest<CreateReviewCommandResponse>
    {
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
