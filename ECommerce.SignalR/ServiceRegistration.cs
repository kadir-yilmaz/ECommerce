using ECommerce.Application.Abstractions.Hubs;
using ECommerce.SignalR.HubServices;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.SignalR
{
    public static class ServiceRegistration
    {
        public static void AddSignalRServices(this IServiceCollection collection)
        {
            collection.AddTransient<IProductHubService, ProductHubService>();
            collection.AddTransient<IOrderHubService, OrderHubService>();
            collection.AddSignalR();
        }
    }
}
