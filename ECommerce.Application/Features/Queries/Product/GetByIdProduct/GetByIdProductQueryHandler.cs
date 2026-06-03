using ECommerce.Application.Repositories;
using MediatR;
using P = ECommerce.Domain.Entities;

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
            P.Product product = await _productReadRepository.GetByIdAsync(request.Id, false);
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
                Campaigns = activeCampaigns
            };
        }
    }
}
