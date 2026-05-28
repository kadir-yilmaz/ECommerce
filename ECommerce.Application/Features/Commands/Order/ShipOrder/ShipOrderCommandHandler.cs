using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.DTOs.Order;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Commands.Order.ShipOrder
{
    public class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommandRequest, ShipOrderCommandResponse>
    {
        readonly IOrderService _orderService;
        readonly IMailService _mailService;
        readonly ILogger<ShipOrderCommandHandler> _logger;

        public ShipOrderCommandHandler(
            IOrderService orderService,
            IMailService mailService,
            ILogger<ShipOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _mailService = mailService;
            _logger = logger;
        }

        public async Task<ShipOrderCommandResponse> Handle(ShipOrderCommandRequest request, CancellationToken cancellationToken)
        {
            (bool succeeded, CompletedOrderDTO? orderInfo) = await _orderService.ShipOrderAsync(request.Id, request.CargoCompany, request.TrackingNumber);
            
            if (succeeded && orderInfo != null && !string.IsNullOrEmpty(orderInfo.EMail))
            {
                try
                {
                    await _mailService.SendShippedOrderMailAsync(
                        orderInfo.EMail,
                        orderInfo.OrderCode,
                        orderInfo.OrderDate,
                        orderInfo.Username,
                        request.CargoCompany,
                        request.TrackingNumber
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send shipped order email for order {OrderCode}", orderInfo.OrderCode);
                }
            }

            return new()
            {
                Succeeded = succeeded
            };
        }
    }
}
