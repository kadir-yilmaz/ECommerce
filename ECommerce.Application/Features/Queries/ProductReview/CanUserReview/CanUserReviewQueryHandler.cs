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

namespace ECommerce.Application.Features.Queries.ProductReview.CanUserReview
{
    public class CanUserReviewQueryHandler : IRequestHandler<CanUserReviewQueryRequest, CanUserReviewQueryResponse>
    {
    private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly IBasketReadRepository _basketReadRepository;
        private readonly IBasketItemReadRepository _basketItemReadRepository;
        private readonly ICompletedOrderReadRepository _completedOrderReadRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CanUserReviewQueryHandler(
            IProductReviewReadRepository reviewReadRepository,
            IOrderReadRepository orderReadRepository,
            IBasketReadRepository basketReadRepository,
            IBasketItemReadRepository basketItemReadRepository,
            ICompletedOrderReadRepository completedOrderReadRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _reviewReadRepository = reviewReadRepository;
            _orderReadRepository = orderReadRepository;
            _basketReadRepository = basketReadRepository;
            _basketItemReadRepository = basketItemReadRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CanUserReviewQueryResponse> Handle(CanUserReviewQueryRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return new CanUserReviewQueryResponse { CanReview = false, Reason = "Giriş yapmanız gerekmektedir." };

            // Teslim edilmiş (Status == Delivered) veya Tamamlanmış (CompletedOrder kaydı olan) sipariş kontrolü
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
                return new CanUserReviewQueryResponse { CanReview = false, Reason = "Bu ürünü satın alıp teslim almadan yorum yapamazsınız." };

            // Mevcut yorum kontrolü
            var existingReview = await _reviewReadRepository.GetAll(false)
                .AnyAsync(r => r.ProductId == request.ProductId && r.UserId == userId && !r.IsDeleted, cancellationToken);

            if (existingReview)
                return new CanUserReviewQueryResponse { CanReview = false, Reason = "Bu ürün için zaten bir yorumunuz bulunmaktadır." };

            return new CanUserReviewQueryResponse { CanReview = true };
        }
    }
}
