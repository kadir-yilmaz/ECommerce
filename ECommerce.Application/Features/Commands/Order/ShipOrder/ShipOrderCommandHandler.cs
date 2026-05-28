using ECommerce.Application.Abstractions.Services;
using MediatR;

namespace ECommerce.Application.Features.Commands.Order.ShipOrder
{
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommandRequest, ShipOrderCommandResponse>
    {
        readonly IOrderService _orderService;

        public ShipOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<ShipOrderCommandResponse> Handle(ShipOrderCommandRequest request, CancellationToken cancellationToken)
        {
            bool succeeded = await _orderService.ShipOrderAsync(request.Id, request.CargoCompany, request.TrackingNumber);
            return new()
            {
                Succeeded = succeeded
            };
        }
    }
}
