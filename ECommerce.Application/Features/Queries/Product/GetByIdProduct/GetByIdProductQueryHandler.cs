using ECommerce.Application.Repositories;
using MediatR;
using P = ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ECommerce.Application.Features.Queries.Product.GetByIdProduct
{
    public class GetByIdProductQueryHandler : IRequestHandler<GetByIdProductQueryRequest, GetByIdProductQueryResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly ECommerce.Application.Repositories.Campaign.ICampaignReadRepository _campaignReadRepository;
        public GetByIdProductQueryHandler(IProductReadRepository productReadRepository, ECommerce.Application.Repositories.Campaign.ICampaignReadRepository campaignReadRepository)
        {
            _productReadRepository = productReadRepository;
            _campaignReadRepository = campaignReadRepository;
        }

        public async Task<GetByIdProductQueryResponse> Handle(GetByIdProductQueryRequest request, CancellationToken cancellationToken)
        {
            Guid productGuid = Guid.Parse(request.Id);
            P.Product product = await _productReadRepository.Table
                .AsNoTracking()
                .Include(p => p.ProductImageFiles)
                .FirstOrDefaultAsync(p => p.Id == productGuid, cancellationToken);
                
            if (product == null)
                throw new System.Collections.Generic.KeyNotFoundException("Product not found");

            var activeCampaigns = _campaignReadRepository.GetAll(false)
                .Where(c => c.IsActive && 
                            ((c.ProductId != null && c.ProductId == product.Id.ToString()) || 
                             (c.CategoryId != null && c.CategoryId == product.CategoryId.ToString())))
                .Select(c => new ECommerce.Application.DTOs.Product.ProductCampaignDto
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Description = c.Description,
                    RuleType = c.RuleType,
                    DiscountRate = c.DiscountRate
                }).ToList();

            return new()
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                ShowOnHomepage = product.ShowOnHomepage,
                CategoryId = product.CategoryId,
                Brand = product.Brand,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                Campaigns = activeCampaigns,
                ImagePath = product.ProductImageFiles?.FirstOrDefault(p => p.Showcase)?.Path 
                            ?? product.ProductImageFiles?.FirstOrDefault()?.Path,
                ProductImageFiles = product.ProductImageFiles?.Select(pif => new GetByIdProductImageResponse
                {
                    Id = pif.Id.ToString(),
                    Path = pif.Path,
                    FileName = pif.FileName,
                    Showcase = pif.Showcase
                }).ToList() ?? new System.Collections.Generic.List<GetByIdProductImageResponse>()
            };
        }
    }
}
