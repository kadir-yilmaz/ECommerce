using ECommerce.Application.Repositories;
using ECommerce.Application.Repositories.ProductReview;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.ProductReview.DeleteReview
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommandRequest, DeleteReviewCommandResponse>
    {
        private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IProductReviewWriteRepository _reviewWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteReviewCommandHandler(
            IProductReviewReadRepository reviewReadRepository,
            IProductReviewWriteRepository reviewWriteRepository,
            IProductReadRepository productReadRepository,
            IProductWriteRepository productWriteRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _reviewReadRepository = reviewReadRepository;
            _reviewWriteRepository = reviewWriteRepository;
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DeleteReviewCommandResponse> Handle(DeleteReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

            var review = await _reviewReadRepository.GetAll()
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId && !r.IsDeleted, cancellationToken);

            if (review == null)
                throw new Exception("Yorum bulunamadı.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("Yalnızca kendi yorumunuzu silebilirsiniz.");

            // Soft delete
            review.IsDeleted = true;
            review.DeletedDate = DateTime.UtcNow;
            _reviewWriteRepository.Update(review);

            // Eğer onaylı bir yorumdu ise ürün istatistiklerini güncelle
            if (review.Status == (int)ReviewStatus.Approved)
            {
                await UpdateProductStats(review.ProductId, cancellationToken);
            }

            await _reviewWriteRepository.SaveAsync();
            return new DeleteReviewCommandResponse { Succeeded = true };
        }

        private async Task UpdateProductStats(Guid productId, CancellationToken cancellationToken)
        {
            var product = await _productReadRepository.GetByIdAsync(productId.ToString());
            if (product == null) return;

            var approvedReviews = await _reviewReadRepository.GetAll(false)
                .Where(r => r.ProductId == productId && r.Status == (int)ReviewStatus.Approved && !r.IsDeleted)
                .ToListAsync(cancellationToken);

            product.ReviewCount = approvedReviews.Count;
            product.AverageRating = approvedReviews.Count > 0
                ? (float)approvedReviews.Average(r => r.Rating)
                : 0;

            _productWriteRepository.Update(product);
        }
    }
}
