using MediatR;
using System;

namespace ECommerce.Application.Features.Queries.ProductReview.GetProductReviews
{
    public class GetProductReviewsQueryRequest : IRequest<GetProductReviewsQueryResponse>
    {
        public Guid ProductId { get; set; }
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 10;
    }
}
