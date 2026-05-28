using ECommerce.Application.Abstractions.Storage;
using ECommerce.Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ECommerce.Application.Features.Commands.ProductImageFile.RemoveProductImage
{
    public class RemoveProductImageCommandHandler : IRequestHandler<RemoveProductImageCommandRequest, RemoveProductImageCommandResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly IProductWriteRepository _productWriteRepository;
        readonly IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly IStorageService _storageService;

        public RemoveProductImageCommandHandler(
            IProductReadRepository productReadRepository, 
            IProductWriteRepository productWriteRepository,
            IProductImageFileWriteRepository productImageFileWriteRepository,
            IStorageService storageService)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _storageService = storageService;
        }

        public async Task<RemoveProductImageCommandResponse> Handle(RemoveProductImageCommandRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Product? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles)
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.Id));

            Domain.Entities.ProductImageFile? productImageFile = product?.ProductImageFiles.FirstOrDefault(p => p.Id == Guid.Parse(request.ImageId));

            if (productImageFile != null)
            {
                // Remove relationship from product
                product?.ProductImageFiles.Remove(productImageFile);

                // Remove database record from ProductImageFiles table
                _productImageFileWriteRepository.Remove(productImageFile);

                // Delete physical file from wwwroot or storage
                string directory = Path.GetDirectoryName(productImageFile.Path) ?? "product-images";
                await _storageService.DeleteAsync(directory, productImageFile.FileName);
            }

            await _productWriteRepository.SaveAsync();
            return new();
        }
    }
}
