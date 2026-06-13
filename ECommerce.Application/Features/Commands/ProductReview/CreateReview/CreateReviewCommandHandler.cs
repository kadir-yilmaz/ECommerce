using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Repositories;
using ECommerce.Application.Repositories.ProductReview;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.ProductReview.CreateReview
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommandRequest, CreateReviewCommandResponse>
    {
    private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IProductReviewWriteRepository _reviewWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly IBasketReadRepository _basketReadRepository;
        private readonly IBasketItemReadRepository _basketItemReadRepository;
        private readonly ICompletedOrderReadRepository _completedOrderReadRepository;
        private readonly IContentModerationService _contentModerationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateReviewCommandHandler(
            IProductReviewReadRepository reviewReadRepository,
            IProductReviewWriteRepository reviewWriteRepository,
            IProductReadRepository productReadRepository,
            IOrderReadRepository orderReadRepository,
            IBasketReadRepository basketReadRepository,
            IBasketItemReadRepository basketItemReadRepository,
            ICompletedOrderReadRepository completedOrderReadRepository,
            IContentModerationService contentModerationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _reviewReadRepository = reviewReadRepository;
            _reviewWriteRepository = reviewWriteRepository;
            _productReadRepository = productReadRepository;
            _orderReadRepository = orderReadRepository;
            _basketReadRepository = basketReadRepository;
            _basketItemReadRepository = basketItemReadRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
            _contentModerationService = contentModerationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateReviewCommandResponse> Handle(CreateReviewCommandRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

            // Ürün var mı?
            var product = await _productReadRepository.GetByIdAsync(request.ProductId.ToString(), false);
            if (product == null)
                throw new Exception("Ürün bulunamadı.");

            // Kullanıcı bu ürünü satın alıp teslim almış/tamamlamış mı?
            var hasDeliveredOrder = await _orderReadRepository.GetAll(false)
                .GroupJoin(_completedOrderReadRepository.GetAll(false),
                    o => o.Id,
                    co => co.OrderId,
                    (o, cos) => new { Order = o, CompletedOrders = cos })
                .SelectMany(x => x.CompletedOrders.DefaultIfEmpty(),
                    (x, co) => new { x.Order, IsCompleted = co != null })
                .Where(x => x.Order.Status == (int)OrderStatus.Delivered || x.IsCompleted)
                .Select(x => x.Order)
                .Join(_basketReadRepository.GetAll(false).Where(b => b.UserId == userId),
                    o => o.Id,
                    b => b.Order != null ? b.Order.Id : Guid.Empty,
                    (o, b) => b)
                .Join(_basketItemReadRepository.GetAll(false).Where(bi => bi.ProductId == request.ProductId),
                    b => b.Id,
                    bi => bi.BasketId,
                    (b, bi) => bi)
                .AnyAsync(cancellationToken);

            if (!hasDeliveredOrder)
                throw new Exception("Yalnızca ürünü satın alıp teslim almış kullanıcılar yorum yapabilir.");

            // Aynı ürüne daha önce yorum yapmış mı?
            var existingReview = await _reviewReadRepository.GetAll(false)
                .FirstOrDefaultAsync(r => r.ProductId == request.ProductId && r.UserId == userId && !r.IsDeleted, cancellationToken);

            if (existingReview != null)
                throw new Exception("Bu ürün için zaten bir yorumunuz bulunmaktadır.");

            // İçerik analizi
            var analysisResult = _contentModerationService.Analyze(request.Comment);

            var review = new Domain.Entities.ProductReview
            {
                ProductId = request.ProductId,
                UserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                Status = (int)ReviewStatus.Pending,
                IsDeleted = false,
                HasProfanity = analysisResult.HasProfanity,
                HasPriceInfo = analysisResult.HasPriceInfo
            };

            await _reviewWriteRepository.AddAsync(review);
            await _reviewWriteRepository.SaveAsync();

            return new CreateReviewCommandResponse
            {
                Id = review.Id.ToString(),
                HasProfanity = analysisResult.HasProfanity,
                HasPriceInfo = analysisResult.HasPriceInfo
            };
        }
    }
}
