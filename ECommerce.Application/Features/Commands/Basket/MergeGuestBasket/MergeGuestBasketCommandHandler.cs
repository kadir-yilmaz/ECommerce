using ECommerce.Application.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Basket.MergeGuestBasket
{
    public class MergeGuestBasketCommandHandler : IRequestHandler<MergeGuestBasketCommandRequest, MergeGuestBasketCommandResponse>
    {
        readonly IBasketService _basketService;
        readonly IDistributedCache _distributedCache;

        public MergeGuestBasketCommandHandler(IBasketService basketService, IDistributedCache distributedCache)
        {
            _basketService = basketService;
            _distributedCache = distributedCache;
        }

        public async Task<MergeGuestBasketCommandResponse> Handle(MergeGuestBasketCommandRequest request, CancellationToken cancellationToken)
        {
            // If guest basket ID is provided in request body, temporarily store it for the service
            if (!string.IsNullOrEmpty(request.GuestBasketId))
            {
                await _basketService.MergeGuestBasketAsync(request.GuestBasketId);
            }
            else
            {
                await _basketService.MergeGuestBasketAsync();
            }
            
            return new MergeGuestBasketCommandResponse();
        }
    }
}
