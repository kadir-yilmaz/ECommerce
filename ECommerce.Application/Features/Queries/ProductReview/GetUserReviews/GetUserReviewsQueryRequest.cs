using MediatR;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Queries.ProductReview.GetUserReviews
{
    public class GetUserReviewsQueryRequest : IRequest<List<GetUserReviewsQueryResponse>>
    {
    }
}
