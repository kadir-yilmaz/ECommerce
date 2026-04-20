namespace ECommerce.Application.Features.Queries.Order.GetOrdersByUser
{
    public class GetOrdersByUserQueryResponse
    {
        public int TotalOrderCount { get; set; }
        public object Orders { get; set; }
    }
}
