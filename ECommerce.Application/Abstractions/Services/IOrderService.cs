using ECommerce.Application.DTOs.Order;

namespace ECommerce.Application.Abstractions.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CreateOrder createOrder);
        Task<ListOrder> GetAllOrdersAsync(int page, int size);
        Task<SingleOrder> GetOrderByIdAsync(string id);
        Task<ListOrder> GetOrdersByUserAsync(string userId, int page, int size);
        Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id);
        Task<bool> DeleteOrderAsync(string id);
        Task<bool> UpdateOrderStatusAsync(string orderId, int status);
        Task<bool> ShipOrderAsync(string orderId, string cargoCompany, string trackingNumber);
    }
}
