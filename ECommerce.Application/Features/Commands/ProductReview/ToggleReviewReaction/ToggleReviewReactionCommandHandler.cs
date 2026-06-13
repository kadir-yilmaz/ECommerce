using ECommerce.Application.Repositories.ProductReview;
using ECommerce.Application.Repositories.ProductReviewReaction;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.ProductReview.ToggleReviewReaction
{
    public class ToggleReviewReactionCommandHandler : IRequestHandler<ToggleReviewReactionCommandRequest, ToggleReviewReactionCommandResponse>
    {
        private readonly IProductReviewReactionReadRepository _reactionReadRepository;
        private readonly IProductReviewReactionWriteRepository _reactionWriteRepository;
        private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ToggleReviewReactionCommandHandler(
            IProductReviewReactionReadRepository reactionReadRepository,
            IProductReviewReactionWriteRepository reactionWriteRepository,
            IProductReviewReadRepository reviewReadRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _reactionReadRepository = reactionReadRepository;
            _reactionWriteRepository = reactionWriteRepository;
            _reviewReadRepository = reviewReadRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ToggleReviewReactionCommandResponse> Handle(ToggleReviewReactionCommandRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new ToggleReviewReactionCommandResponse
                {
                    Success = false,
                    Message = "Oturum açmış olmanız gerekmektedir."
                };
            }

            // Yorumun varlığını ve silinmediğini kontrol et
            var reviewExists = await _reviewReadRepository.Table.AnyAsync(r => r.Id == request.ReviewId && !r.IsDeleted, cancellationToken);
            if (!reviewExists)
            {
                return new ToggleReviewReactionCommandResponse
                {
                    Success = false,
                    Message = "Yorum bulunamadı."
                };
            }

            // Kullanıcının bu yoruma tepkisi var mı kontrol et
            var existingReaction = await _reactionReadRepository.Table
                .FirstOrDefaultAsync(r => r.ReviewId == request.ReviewId && r.UserId == userId, cancellationToken);

            string message;
            string? currentUserReaction = null;

            if (existingReaction != null)
            {
                if (existingReaction.IsLike == request.IsLike)
                {
                    // Aynı reaksiyon tıklanmış, reaksiyonu geri çek (sil)
                    _reactionWriteRepository.Remove(existingReaction);
                    message = "Reaksiyonunuz geri çekildi.";
                }
                else
                {
                    // Farklı reaksiyon tıklanmış, güncelle
                    existingReaction.IsLike = request.IsLike;
                    _reactionWriteRepository.Update(existingReaction);
                    message = request.IsLike ? "Yorumu beğendiniz." : "Yorumu beğenmediniz.";
                    currentUserReaction = request.IsLike ? "like" : "dislike";
                }
            }
            else
            {
                // Reaksiyon yok, yeni reaksiyon ekle
                var newReaction = new ProductReviewReaction
                {
                    ReviewId = request.ReviewId,
                    UserId = userId,
                    IsLike = request.IsLike
                };
                await _reactionWriteRepository.AddAsync(newReaction);
                message = request.IsLike ? "Yorumu beğendiniz." : "Yorumu beğenmediniz.";
                currentUserReaction = request.IsLike ? "like" : "dislike";
            }

            await _reactionWriteRepository.SaveAsync();

            // Güncel Like/Dislike sayılarını hesapla
            var reactionsQuery = _reactionReadRepository.Table.Where(r => r.ReviewId == request.ReviewId);
            var likes = await reactionsQuery.CountAsync(r => r.IsLike, cancellationToken);
            var dislikes = await reactionsQuery.CountAsync(r => !r.IsLike, cancellationToken);

            return new ToggleReviewReactionCommandResponse
            {
                Success = true,
                LikeCount = likes,
                DislikeCount = dislikes,
                CurrentUserReaction = currentUserReaction,
                Message = message
            };
        }
    }
}
