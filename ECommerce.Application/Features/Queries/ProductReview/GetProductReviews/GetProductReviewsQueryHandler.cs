using ECommerce.Application.Repositories.ProductReview;
using ECommerce.Application.Repositories.ProductReviewReaction;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.ProductReview.GetProductReviews
{
    public class GetProductReviewsQueryHandler : IRequestHandler<GetProductReviewsQueryRequest, GetProductReviewsQueryResponse>
    {
        private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IProductReviewReactionReadRepository _reactionReadRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetProductReviewsQueryHandler(
            IProductReviewReadRepository reviewReadRepository,
            IProductReviewReactionReadRepository reactionReadRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _reviewReadRepository = reviewReadRepository;
            _reactionReadRepository = reactionReadRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetProductReviewsQueryResponse> Handle(GetProductReviewsQueryRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var query = _reviewReadRepository.GetAll(false)
                .Where(r => r.ProductId == request.ProductId
                    && r.Status == (int)ReviewStatus.Approved
                    && !r.IsDeleted);

            var totalCount = await query.CountAsync(cancellationToken);

            // Rating dağılımı
            var ratingDistribution = new Dictionary<int, int>
            {
                { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }
            };

            var ratingGroups = await query
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            foreach (var group in ratingGroups)
            {
                if (ratingDistribution.ContainsKey(group.Rating))
                    ratingDistribution[group.Rating] = group.Count;
            }

            var averageRating = totalCount > 0
                ? (float)await query.AverageAsync(r => r.Rating, cancellationToken)
                : 0;

            var reactionsQuery = _reactionReadRepository.Table;

            var reviews = await query
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedDate)
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .Select(r => new
                {
                    r.Id,
                    r.ProductId,
                    r.UserId,
                    UserName = r.User != null ? r.User.NameSurname : "Anonim",
                    r.Rating,
                    r.Comment,
                    r.CreatedDate,
                    r.UpdatedDate,
                    LikeCount = reactionsQuery.Count(x => x.ReviewId == r.Id && x.IsLike),
                    DislikeCount = reactionsQuery.Count(x => x.ReviewId == r.Id && !x.IsLike),
                    CurrentUserReaction = userId == null ? null : reactionsQuery
                        .Where(x => x.ReviewId == r.Id && x.UserId == userId)
                        .Select(x => x.IsLike ? "like" : "dislike")
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            return new GetProductReviewsQueryResponse
            {
                Reviews = reviews,
                TotalCount = totalCount,
                AverageRating = averageRating,
                RatingDistribution = ratingDistribution
            };
        }
    }
}
