using MediatR;

namespace ECommerce.Application.Features.Queries.Order.GetOrdersByUser
{
    public class GetOrdersByUserQueryRequest : IRequest<GetOrdersByUserQueryResponse>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 5;
    }
}
