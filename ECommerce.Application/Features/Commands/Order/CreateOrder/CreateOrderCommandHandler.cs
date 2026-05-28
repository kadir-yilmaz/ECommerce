using ECommerce.Application.Abstractions.Hubs;
using ECommerce.Application.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Order.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommandRequest, CreateOrderCommandResponse>
    {
        readonly IOrderService _orderService;
        readonly IOrderHubService _orderHubService;
        readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(IOrderService orderService, IOrderHubService orderHubService, ILogger<CreateOrderCommandHandler> logger)
        {
            _orderService = orderService;
            _orderHubService = orderHubService;
            _logger = logger;
        }

        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommandRequest request, CancellationToken cancellationToken)
        {
            var cardLast4 = request.CardNumber.Length >= 4
                ? request.CardNumber[^4..]
                : request.CardNumber;

            _logger.LogInformation(
                "Checkout request received for {City}/{District}. Card ending with {CardLast4}.",
                request.City,
                request.District,
                cardLast4);

            var (succeeded, errorMessage) = await _orderService.CreateOrderAsync(new()
            {
                BasketId = request.BasketId,
                Description = request.Description,
                ContactName = request.ContactName,
                PhoneNumber = request.PhoneNumber,
                City = request.City,
                District = request.District,
                Neighborhood = request.Neighborhood,
                PostalCode = request.PostalCode,
                AddressLine = request.AddressLine,
                CardHolderName = request.CardHolderName,
                CardNumber = request.CardNumber,
                ExpireMonth = request.ExpireMonth,
                ExpireYear = request.ExpireYear,
                Cvv = request.Cvv
            });

            if (!succeeded)
            {
                _logger.LogWarning("CreateOrder failed: {ErrorMessage}", errorMessage);
                return new() { Succeeded = false, Message = errorMessage };
            }

            await _orderHubService.OrderAddedMessageAsync("Heyy, yeni bir sipariş geldi! :) ");

            return new() { Succeeded = true, Message = "Siparişiniz başarıyla oluşturuldu." };
        }
    }
}
