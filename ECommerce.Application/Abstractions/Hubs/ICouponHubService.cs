namespace ECommerce.Application.Abstractions.Hubs
{
    public interface ICouponHubService
    {
        Task CouponAddedMessageAsync(string message, string userId);
    }
}
