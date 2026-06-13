using ECommerce.Application.Repositories.ProductReview;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.ProductReview.GetUserReview
{
    public class GetUserReviewQueryHandler : IRequestHandler<GetUserReviewQueryRequest, GetUserReviewQueryResponse>
    {
        private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserReviewQueryHandler(IProductReviewReadRepository reviewReadRepository, IHttpContextAccessor httpContextAccessor)
        {
            _reviewReadRepository = reviewReadRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetUserReviewQueryResponse> Handle(GetUserReviewQueryRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return new GetUserReviewQueryResponse { HasReview = false };

            var review = await _reviewReadRepository.GetAll(false)
                .Where(r => r.ProductId == request.ProductId && r.UserId == userId && !r.IsDeleted)
                .Select(r => new
                {
                    r.Id,
                    r.ProductId,
                    r.UserId,
                    r.Rating,
                    r.Comment,
                    r.Status,
                    r.AdminNote,
                    r.CreatedDate,
                    r.UpdatedDate
                })
                .FirstOrDefaultAsync(cancellationToken);

            return new GetUserReviewQueryResponse
            {
                HasReview = review != null,
                Review = review
            };
        }
    }
}
