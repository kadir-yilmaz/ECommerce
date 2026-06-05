using ECommerce.Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Queries.Dashboard.GetDashboardOverview
{
    public class GetDashboardOverviewQueryHandler : IRequestHandler<GetDashboardOverviewQueryRequest, GetDashboardOverviewQueryResponse>
    {
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly UserManager<ECommerce.Domain.Entities.AppUser> _userManager;

        public GetDashboardOverviewQueryHandler(
            IOrderReadRepository orderReadRepository,
            IProductReadRepository productReadRepository,
            UserManager<ECommerce.Domain.Entities.AppUser> userManager)
        {
            _orderReadRepository = orderReadRepository;
            _productReadRepository = productReadRepository;
            _userManager = userManager;
        }

        public async Task<GetDashboardOverviewQueryResponse> Handle(GetDashboardOverviewQueryRequest request, CancellationToken cancellationToken)
        {
            // 1. Total Orders
            int totalOrders = await _orderReadRepository.GetAll(false).CountAsync(cancellationToken);

            // 2. Total Products
            int totalProducts = await _productReadRepository.GetAll(false).CountAsync(cancellationToken);

            // 3. Total Users
            int totalUsers = await _userManager.Users.CountAsync(cancellationToken);

            // 4. Total Sales
            // Get all orders with basket items to calculate total sales
            var orders = await _orderReadRepository.GetAll(false)
                .Include(o => o.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product)
                .Include(o => o.OrderDiscounts)
                .ToListAsync(cancellationToken);

            decimal totalSales = 0;
            foreach (var order in orders)
            {
                if (order.Basket != null && order.Basket.BasketItems != null)
                {
                    var basePrice = order.Basket.BasketItems.Sum(bi => (decimal)(bi.Product.Price * bi.Quantity));
                    var totalDiscount = order.OrderDiscounts?.Sum(d => d.DiscountAmount) ?? 0m;
                    totalSales += basePrice - totalDiscount;
                }
            }


            return new GetDashboardOverviewQueryResponse
            {
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                TotalSales = totalSales
            };
        }
    }
}
