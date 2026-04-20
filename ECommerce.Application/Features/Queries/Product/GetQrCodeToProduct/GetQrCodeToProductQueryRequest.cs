using MediatR;

namespace ECommerce.Application.Features.Queries.Product.GetQrCodeToProduct
{
    public class GetQrCodeToProductQueryRequest : IRequest<GetQrCodeToProductQueryResponse>
    {
        public string ProductId { get; set; }
    }
}
