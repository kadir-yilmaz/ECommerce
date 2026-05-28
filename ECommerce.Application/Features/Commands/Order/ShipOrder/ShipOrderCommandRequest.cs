using MediatR;

namespace ECommerce.Application.Features.Commands.Order.ShipOrder
{
    public class ShipOrderCommandRequest : IRequest<ShipOrderCommandResponse>
    {
        public string Id { get; set; } = string.Empty;
        public string CargoCompany { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
    }
}
