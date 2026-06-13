using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Repositories.ProductReview;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.ProductReview.UpdateReview
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommandRequest, UpdateReviewCommandResponse>
    {
        private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IProductReviewWriteRepository _reviewWriteRepository;
        private readonly IContentModerationService _contentModerationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateReviewCommandHandler(
            IProductReviewReadRepository reviewReadRepository,
            IProductReviewWriteRepository reviewWriteRepository,
            IContentModerationService contentModerationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _reviewReadRepository = reviewReadRepository;
            _reviewWriteRepository = reviewWriteRepository;
            _contentModerationService = contentModerationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UpdateReviewCommandResponse> Handle(UpdateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

            var review = await _reviewReadRepository.GetAll()
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId && !r.IsDeleted, cancellationToken);

            if (review == null)
                throw new Exception("Yorum bulunamadı.");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("Yalnızca kendi yorumunuzu güncelleyebilirsiniz.");

            // İçerik analizi
            var analysisResult = _contentModerationService.Analyze(request.Comment);

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            review.Status = (int)ReviewStatus.Pending; // Güncellenmiş yorum tekrar onay bekler
            review.HasProfanity = analysisResult.HasProfanity;
            review.HasPriceInfo = analysisResult.HasPriceInfo;

            _reviewWriteRepository.Update(review);
            await _reviewWriteRepository.SaveAsync();

            return new UpdateReviewCommandResponse { Succeeded = true };
        }
    }
}
