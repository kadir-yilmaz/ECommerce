using MediatR;
using System;

namespace ECommerce.Application.Features.Queries.ProductReview.CanUserReview
{
    public class CanUserReviewQueryRequest : IRequest<CanUserReviewQueryResponse>
    {
        public Guid ProductId { get; set; }
    }
}
