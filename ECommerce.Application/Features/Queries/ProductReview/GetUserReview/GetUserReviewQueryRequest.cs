using MediatR;
using System;

namespace ECommerce.Application.Features.Queries.ProductReview.GetUserReview
{
    public class GetUserReviewQueryRequest : IRequest<GetUserReviewQueryResponse>
    {
        public Guid ProductId { get; set; }
    }
}
