using System;

namespace ECommerce.Application.Features.Queries.ProductReview.GetUserReviews
{
    public class GetUserReviewsQueryResponse
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductBrand { get; set; }
        public string ProductImagePath { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
