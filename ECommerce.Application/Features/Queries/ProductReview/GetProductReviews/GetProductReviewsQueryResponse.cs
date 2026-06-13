using System.Collections.Generic;

namespace ECommerce.Application.Features.Queries.ProductReview.GetProductReviews
{
    public class GetProductReviewsQueryResponse
    {
        public object Reviews { get; set; }
        public int TotalCount { get; set; }
        public float AverageRating { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; }
    }
}
