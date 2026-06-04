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
            var activeCampaigns = _campaignReadRepository.GetWhere(c => c.IsActive && (c.EndDate == null || c.EndDate > DateTime.UtcNow)).ToList();

            return basketItems.Select(ba => new GetBasketItemsQueryResponse
            {
                BasketItemId = ba.Id.ToString(),
                Name = ba.Product.Name,
                Price = ba.Product.Price,
                Quantity = ba.Quantity,
                ProductId = ba.ProductId.ToString(),
                CategoryId = ba.Product.CategoryId?.ToString() ?? "",
                ImagePath = ba.Product.ProductImageFiles?.FirstOrDefault(p => p.Showcase)?.Path 
                            ?? ba.Product.ProductImageFiles?.FirstOrDefault()?.Path,
                Brand = ba.Product.Brand,
                Campaigns = activeCampaigns.Where(c => 
                    (c.ProductId != null && (
                        string.Equals(c.ProductId, ba.ProductId.ToString(), StringComparison.OrdinalIgnoreCase) ||
                        c.ProductId.Split(',', StringSplitOptions.RemoveEmptyEntries).Any(idStr => string.Equals(idStr.Trim(), ba.ProductId.ToString(), StringComparison.OrdinalIgnoreCase))
                    )) || 
                    (c.RuleType == "CategoryDiscount" && c.CategoryId != null && ba.Product.CategoryId != null && string.Equals(c.CategoryId, ba.Product.CategoryId.ToString(), StringComparison.OrdinalIgnoreCase) &&
                        (string.IsNullOrEmpty(c.Brand) || (!string.IsNullOrEmpty(ba.Product.Brand) && string.Equals(c.Brand, ba.Product.Brand, StringComparison.OrdinalIgnoreCase)))
                    ) ||
                    (c.RuleType == "BrandDiscount" && !string.IsNullOrEmpty(c.Brand) && !string.IsNullOrEmpty(ba.Product.Brand) && string.Equals(c.Brand, ba.Product.Brand, StringComparison.OrdinalIgnoreCase) &&
                        (string.IsNullOrEmpty(c.CategoryId) || (ba.Product.CategoryId != null && string.Equals(c.CategoryId, ba.Product.CategoryId.ToString(), StringComparison.OrdinalIgnoreCase)))
                    ) ||
                    (c.RuleType != "CategoryDiscount" && c.RuleType != "BrandDiscount" && c.CategoryId != null && ba.Product.CategoryId != null && string.Equals(c.CategoryId, ba.Product.CategoryId.ToString(), StringComparison.OrdinalIgnoreCase))
                ).Select(c => new ECommerce.Application.DTOs.Product.ProductCampaignDto
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Description = c.Description,
                    RuleType = c.RuleType,
                    DiscountRate = c.DiscountRate,
                    MinAmount = c.MinAmount,
                    MinQuantity = c.MinQuantity,
                    FreeQuantity = c.FreeQuantity,
                    ProductId = c.ProductId,
                    CategoryId = c.CategoryId
                }).ToList()
            }).ToList();
        }
    }
}
