using ECommerce.Application.Repositories.ProductReview;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.ProductReview.GetUserReviews
{
    public class GetUserReviewsQueryHandler : IRequestHandler<GetUserReviewsQueryRequest, List<GetUserReviewsQueryResponse>>
    {
        private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserReviewsQueryHandler(
            IProductReviewReadRepository reviewReadRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _reviewReadRepository = reviewReadRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<GetUserReviewsQueryResponse>> Handle(GetUserReviewsQueryRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

            var reviews = await _reviewReadRepository.Table
                .AsNoTracking()
                .IgnoreQueryFilters() // Soft-deleted (IsDeleted = true) olanları da çekmek için
                .Where(r => r.UserId == userId)
                .Include(r => r.Product)
                    .ThenInclude(p => p.ProductImageFiles)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync(cancellationToken);

            return reviews.Select(r => new GetUserReviewsQueryResponse
            {
                Id = r.Id.ToString(),
                ProductId = r.ProductId.ToString(),
                ProductName = r.Product?.Name ?? string.Empty,
                ProductBrand = r.Product?.Brand ?? string.Empty,
                ProductImagePath = r.Product?.ProductImageFiles?.FirstOrDefault(p => p.Showcase)?.Path 
                                   ?? r.Product?.ProductImageFiles?.FirstOrDefault()?.Path 
                                   ?? string.Empty,
                Rating = r.Rating,
                Comment = r.Comment,
                Status = r.Status,
                IsDeleted = r.IsDeleted,
                CreatedDate = r.CreatedDate,
                UpdatedDate = r.UpdatedDate
            }).ToList();
        }
    }
}
