using ECommerce.Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Features.Commands.Product.ChangeShowcaseProduct
{
    public class ChangeShowcaseProductCommandHandler : IRequestHandler<ChangeShowcaseProductCommandRequest, ChangeShowcaseProductCommandResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductWriteRepository _productWriteRepository;
        readonly ILogger<ChangeShowcaseProductCommandHandler> _logger;

        public ChangeShowcaseProductCommandHandler(IProductReadRepository productReadRepository, IProductWriteRepository productWriteRepository, ILogger<ChangeShowcaseProductCommandHandler> logger)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _logger = logger;
        }

        public async Task<ChangeShowcaseProductCommandResponse> Handle(ChangeShowcaseProductCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Product product = await _productReadRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new System.Collections.Generic.KeyNotFoundException("Product not found");

            product.ShowOnHomepage = request.ShowOnHomepage;
            await _productWriteRepository.SaveAsync();
            
            _logger.LogInformation("Product showcase status updated to {status}", request.ShowOnHomepage);
            
            return new();
        }
    }
}
