using MediatR;

namespace ECommerce.Application.Features.Queries.ProductReview.GetAllReviewsForAdmin
{
    public class GetAllReviewsForAdminQueryRequest : IRequest<GetAllReviewsForAdminQueryResponse>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 20;
        public int? Status { get; set; }
    }
}
