using MediatR;

namespace ECommerce.Application.Features.Commands.Order.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandRequest : IRequest<UpdateOrderStatusCommandResponse>
    {
        public string Id { get; set; }
        public int Status { get; set; }
    }
}
