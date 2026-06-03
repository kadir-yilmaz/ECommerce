using ECommerce.Application.Abstractions.Services;
using ECommerce.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Repositories.Campaign;

namespace ECommerce.Application.Features.Queries.Basket.GetBasketItems
{
    public class GetBasketItemsQueryHandler : IRequestHandler<GetBasketItemsQueryRequest, List<GetBasketItemsQueryResponse>>
    {
        readonly IBasketService _basketService;
        readonly ICampaignReadRepository _campaignReadRepository;

        public GetBasketItemsQueryHandler(IBasketService basketService, ICampaignReadRepository campaignReadRepository)
        {
            _basketService = basketService;
            _campaignReadRepository = campaignReadRepository;
        }

        public async Task<List<GetBasketItemsQueryResponse>> Handle(GetBasketItemsQueryRequest request, CancellationToken cancellationToken)
        {
            List<BasketItem> basketItems = await _basketService.GetBasketItemsAsync();
            var activeCampaigns = _campaignReadRepository.GetWhere(c => c.IsActive).ToList();

            return basketItems.Select(ba => new GetBasketItemsQueryResponse
            {
                BasketItemId = ba.Id.ToString(),
                Name = ba.Product.Name,
                Price = ba.Product.Price,
                Quantity = ba.Quantity,
                ProductId = ba.ProductId.ToString(),
                CategoryId = ba.Product.CategoryId?.ToString() ?? "",
                Campaigns = activeCampaigns.Where(c => 
                    (c.ProductId != null && c.ProductId == ba.ProductId.ToString()) || 
                    (c.CategoryId != null && ba.Product.CategoryId != null && c.CategoryId == ba.Product.CategoryId.ToString())
                ).Select(c => new ECommerce.Application.DTOs.Product.ProductCampaignDto
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Description = c.Description,
                    RuleType = c.RuleType,
                    DiscountRate = c.DiscountRate
                }).ToList()
            }).ToList();
        }
    }
}
