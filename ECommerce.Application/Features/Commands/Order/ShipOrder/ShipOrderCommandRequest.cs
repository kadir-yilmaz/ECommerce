using MediatR;

namespace ECommerce.Application.Features.Commands.Order.ShipOrder
{
    public class ShipOrderCommandRequest : IRequest<ShipOrderCommandResponse>
    {
        public string Id { get; set; }
        public string CargoCompany { get; set; }
        public string TrackingNumber { get; set; }
    }
}
