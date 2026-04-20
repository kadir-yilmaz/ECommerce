using MediatR;

namespace ECommerce.Application.Features.Commands.Order.RemoveOrder
{
    public class RemoveOrderCommandRequest : IRequest<RemoveOrderCommandResponse>
    {
        public string Id { get; set; }
    }
}
