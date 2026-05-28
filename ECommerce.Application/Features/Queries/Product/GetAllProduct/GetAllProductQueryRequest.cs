using MediatR;

namespace ECommerce.Application.Features.Queries.Product.GetAllProduct
{
    public class GetAllProductQueryRequest : IRequest<GetAllProductQueryResponse>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 5;
        public System.Guid? CategoryId { get; set; }
        public string? SortType { get; set; }
        public string? Search { get; set; }
    }
}
