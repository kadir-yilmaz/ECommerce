using System;

namespace ECommerce.Application.Features.Queries.ProductReview.GetUserReview
{
    public class GetUserReviewQueryResponse
    {
        public bool HasReview { get; set; }
        public object? Review { get; set; }
    }
}
