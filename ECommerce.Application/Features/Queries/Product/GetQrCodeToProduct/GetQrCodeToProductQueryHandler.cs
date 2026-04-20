using ECommerce.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Product.GetQrCodeToProduct
{
    public class GetQrCodeToProductQueryHandler : IRequestHandler<GetQrCodeToProductQueryRequest, GetQrCodeToProductQueryResponse>
    {
        readonly IProductService _productService;

        public GetQrCodeToProductQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<GetQrCodeToProductQueryResponse> Handle(GetQrCodeToProductQueryRequest request, CancellationToken cancellationToken)
        {
            var data = await _productService.QrCodeToProductAsync(request.ProductId);
            return new()
            {
                QrCode = data
            };
        }
    }
}
