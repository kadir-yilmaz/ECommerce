using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.DTOs.Order;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ECommerce.Application.Features.Queries.Order.GetOrdersByUser
{
    public class GetOrdersByUserQueryHandler : IRequestHandler<GetOrdersByUserQueryRequest, GetOrdersByUserQueryResponse>
    {
        readonly IOrderService _orderService;
        readonly IHttpContextAccessor _httpContextAccessor;

        public GetOrdersByUserQueryHandler(IOrderService orderService, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetOrdersByUserQueryResponse> Handle(GetOrdersByUserQueryRequest request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");
            }

            ListOrder data = await _orderService.GetOrdersByUserAsync(userId, request.Page, request.Size);

            return new()
            {
                TotalOrderCount = data.TotalOrderCount,
                Orders = data.Orders
            };
        }
    }
}
