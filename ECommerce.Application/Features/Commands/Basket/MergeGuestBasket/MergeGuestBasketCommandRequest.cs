using MediatR;

namespace ECommerce.Application.Features.Commands.Basket.MergeGuestBasket
{
    public class MergeGuestBasketCommandRequest : IRequest<MergeGuestBasketCommandResponse>
    {
        public string? GuestBasketId { get; set; }
    }
}
