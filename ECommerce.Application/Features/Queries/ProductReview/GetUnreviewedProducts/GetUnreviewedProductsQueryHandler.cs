using ECommerce.Application.Repositories;
using ECommerce.Application.Repositories.ProductReview;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.ProductReview.GetUnreviewedProducts
{
    public class GetUnreviewedProductsQueryHandler : IRequestHandler<GetUnreviewedProductsQueryRequest, List<GetUnreviewedProductsQueryResponse>>
    {
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly IBasketReadRepository _basketReadRepository;
        private readonly IBasketItemReadRepository _basketItemReadRepository;
        private readonly ICompletedOrderReadRepository _completedOrderReadRepository;
        private readonly IProductReviewReadRepository _reviewReadRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUnreviewedProductsQueryHandler(
            IOrderReadRepository orderReadRepository,
            IBasketReadRepository basketReadRepository,
            IBasketItemReadRepository basketItemReadRepository,
            ICompletedOrderReadRepository completedOrderReadRepository,
            IProductReviewReadRepository reviewReadRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderReadRepository = orderReadRepository;
            _basketReadRepository = basketReadRepository;
            _basketItemReadRepository = basketItemReadRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
            _reviewReadRepository = reviewReadRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<GetUnreviewedProductsQueryResponse>> Handle(GetUnreviewedProductsQueryRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");

            // Kullanıcının teslim edilmiş veya tamamlanmış siparişleri
            var completedOrders = await _orderReadRepository.GetAll(false)
                .GroupJoin(_completedOrderReadRepository.GetAll(false),
                    o => o.Id,
                    co => co.OrderId,
                    (o, cos) => new { Order = o, CompletedOrders = cos })
                .SelectMany(x => x.CompletedOrders.DefaultIfEmpty(),
                    (x, co) => new { x.Order, IsCompleted = co != null })
                .Where(x => x.Order.Status == (int)OrderStatus.Delivered || x.IsCompleted)
                .Join(_basketReadRepository.GetAll(false).Where(b => b.UserId == userId),
                    x => x.Order.Id,
                    b => b.Order != null ? b.Order.Id : Guid.Empty,
                    (x, b) => new { Order = x.Order, BasketId = b.Id })
                .ToListAsync(cancellationToken);

            var basketIds = completedOrders.Select(c => c.BasketId).ToList();

            // Bu sepetlerdeki ürünler
            var deliveredProducts = await _basketItemReadRepository.Table
                .Where(bi => basketIds.Contains(bi.BasketId))
                .Include(bi => bi.Product)
                    .ThenInclude(p => p.ProductImageFiles)
                .Include(bi => bi.Basket)
                    .ThenInclude(b => b.Order)
                .ToListAsync(cancellationToken);

            // Kullanıcının zaten yorum yaptığı ürünlerin ID'leri
            var reviewedProductIds = await _reviewReadRepository.Table
                .Where(r => r.UserId == userId && !r.IsDeleted)
                .Select(r => r.ProductId)
                .ToListAsync(cancellationToken);

            // Yorum yapılmamış ürünleri filtrele ve grupla (aynı ürün farklı siparişlerde olabilir, tekil gösterelim)
            var unreviewed = deliveredProducts
                .Where(dp => !reviewedProductIds.Contains(dp.ProductId) && dp.Product != null)
                .GroupBy(dp => dp.ProductId)
                .Select(g => {
                    var first = g.First();
                    return new GetUnreviewedProductsQueryResponse
                    {
                        ProductId = g.Key.ToString(),
                        ProductName = first.Product.Name,
                        ProductBrand = first.Product.Brand ?? string.Empty,
                        ProductPrice = first.Product.Price,
                        ProductImagePath = first.Product.ProductImageFiles?.FirstOrDefault(p => p.Showcase)?.Path 
                                           ?? first.Product.ProductImageFiles?.FirstOrDefault()?.Path 
                                           ?? string.Empty,
                        DeliveryDate = first.Basket?.Order?.UpdatedDate ?? first.Basket?.Order?.CreatedDate ?? DateTime.UtcNow
                    };
                })
                .OrderByDescending(p => p.DeliveryDate)
                .ToList();

            return unreviewed;
        }
    }
}
