using ECommerce.Application.Abstractions.Hubs;
using ECommerce.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ECommerce.SignalR.HubServices
{
    public class CouponHubService : ICouponHubService
    {
        readonly IHubContext<CouponHub> _hubContext;

        public CouponHubService(IHubContext<CouponHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task CouponAddedMessageAsync(string message, string userId)
        {
            // Note: Sending to a specific user requires UserId provider to be set up correctly in SignalR. 
            // In a typical identity setup, Clients.User(userId) targets the connections of that user.
            await _hubContext.Clients.User(userId).SendAsync(ReceiveFunctionNames.CouponAddedMessage, message);
        }
    }
}
