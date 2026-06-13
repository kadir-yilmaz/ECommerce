using ECommerce.Application.Repositories;
using ECommerce.Application.Repositories.ProductReview;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.ProductReview.ModerateReview
{
    public class ModerateReviewCommandHandler : IRequestHandler<ModerateReviewCommandRequest, ModerateReviewCommandResponse>
    {
        private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IProductReviewWriteRepository _reviewWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;

        public ModerateReviewCommandHandler(
            IProductReviewReadRepository reviewReadRepository,
            IProductReviewWriteRepository reviewWriteRepository,
            IProductReadRepository productReadRepository,
            IProductWriteRepository productWriteRepository)
        {
            _reviewReadRepository = reviewReadRepository;
            _reviewWriteRepository = reviewWriteRepository;
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
        }

        public async Task<ModerateReviewCommandResponse> Handle(ModerateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var review = await _reviewReadRepository.GetAll()
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId && !r.IsDeleted, cancellationToken);

            if (review == null)
                throw new Exception("Yorum bulunamadı.");

            if (request.Status != (int)ReviewStatus.Approved && request.Status != (int)ReviewStatus.Rejected)
                throw new Exception("Geçersiz durum. Sadece Approved (1) veya Rejected (2) kabul edilir.");

            review.Status = request.Status;
            review.AdminNote = request.AdminNote;
            _reviewWriteRepository.Update(review);

            // Ürün istatistiklerini güncelle
            await UpdateProductStats(review.ProductId, cancellationToken);

            await _reviewWriteRepository.SaveAsync();

            return new ModerateReviewCommandResponse { Succeeded = true };
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
