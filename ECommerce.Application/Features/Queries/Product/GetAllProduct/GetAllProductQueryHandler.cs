using ECommerce.Application.Repositories;
using ECommerce.Application.Repositories.Category;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Product.GetAllProduct
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly ICategoryReadRepository _categoryReadRepository;
        readonly ECommerce.Application.Repositories.Campaign.ICampaignReadRepository _campaignReadRepository;
        readonly ILogger<GetAllProductQueryHandler> _logger;

        public GetAllProductQueryHandler(
            IProductReadRepository productReadRepository, 
            ICategoryReadRepository categoryReadRepository,
            ECommerce.Application.Repositories.Campaign.ICampaignReadRepository campaignReadRepository,
            ILogger<GetAllProductQueryHandler> logger)
        {
            _productReadRepository = productReadRepository;
            _categoryReadRepository = categoryReadRepository;
            _campaignReadRepository = campaignReadRepository;
            _logger = logger;
        }

        public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all products");

            var query = _productReadRepository.GetAll(false);

            if (request.CategoryId.HasValue)
            {
                var categoryId = request.CategoryId.Value;
                var allCategories = _categoryReadRepository.GetAll(false).ToList();
                
                var descendantIds = GetDescendantCategoryIds(categoryId, allCategories);
                descendantIds.Add(categoryId);

                query = query.Where(p => p.CategoryId.HasValue && descendantIds.Contains(p.CategoryId.Value));
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search) || (p.Brand != null && p.Brand.ToLower().Contains(search)));
            }

            if (request.IsShowcase == true)
            {
                query = query.Where(p => p.ShowOnHomepage);
            }

            if (!string.IsNullOrWhiteSpace(request.Brand))
            {
                query = query.Where(p => p.Brand != null && p.Brand.ToLower() == request.Brand.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(request.ProductIds))
            {
                var productIdsList = request.ProductIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                query = query.Where(p => productIdsList.Contains(p.Id.ToString()));
            }

            var totalProductCount = query.Count();

            query = request.SortType switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.CreatedDate),
                "bestseller" => query.OrderByDescending(p => p.Stock), // Just a dummy for now
                _ => query.OrderByDescending(p => p.CreatedDate)
            };

            var activeCampaigns = _campaignReadRepository.GetAll(false).Where(c => c.IsActive && (c.EndDate == null || c.EndDate > DateTime.UtcNow)).ToList();

            var productsDb = query
                .Skip(request.Page * request.Size).Take(request.Size)
                .Include(p => p.ProductImageFiles)
                .ToList();

            var products = productsDb.Select(p => new
            {
                p.Id,
                p.Name,
                p.Brand,
                p.Stock,
                p.Price,
                p.CreatedDate,
                p.UpdatedDate,
                p.ShowOnHomepage,
                ProductImageFiles = p.ProductImageFiles.Select(pif => new {
                    pif.Id,
                    pif.Path,
                    pif.FileName,
                    pif.Showcase
                }).ToList(),
                p.CategoryId,
                Campaigns = activeCampaigns.Where(c => 
                    (c.ProductId != null && (
                        (Guid.TryParse(c.ProductId, out var prodId) && prodId == p.Id) ||
                        c.ProductId.Split(',', StringSplitOptions.RemoveEmptyEntries).Any(idStr => Guid.TryParse(idStr.Trim(), out var parsedId) && parsedId == p.Id)
                    )) || 
                    (c.RuleType == "CategoryDiscount" && c.CategoryId != null && Guid.TryParse(c.CategoryId, out var catId) && p.CategoryId.HasValue && catId == p.CategoryId.Value &&
                        (string.IsNullOrEmpty(c.Brand) || (!string.IsNullOrEmpty(p.Brand) && string.Equals(c.Brand, p.Brand, StringComparison.OrdinalIgnoreCase)))
                    ) ||
                    (c.RuleType == "BrandDiscount" && !string.IsNullOrEmpty(c.Brand) && !string.IsNullOrEmpty(p.Brand) && string.Equals(c.Brand, p.Brand, StringComparison.OrdinalIgnoreCase) &&
                        (string.IsNullOrEmpty(c.CategoryId) || (Guid.TryParse(c.CategoryId, out var bCatId) && p.CategoryId.HasValue && bCatId == p.CategoryId.Value))
                    ) ||
                    (c.RuleType != "CategoryDiscount" && c.RuleType != "BrandDiscount" && c.CategoryId != null && Guid.TryParse(c.CategoryId, out var legacyCatId) && p.CategoryId.HasValue && legacyCatId == p.CategoryId.Value)
                ).Select(c => new ECommerce.Application.DTOs.Product.ProductCampaignDto
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    Description = c.Description,
                    RuleType = c.RuleType,
                    DiscountRate = c.DiscountRate
                }).ToList()
            }).ToList();

            return new()
            {
                Products = products,
                TotalProductCount = totalProductCount
            };
        }

        private List<Guid> GetDescendantCategoryIds(Guid parentId, List<ECommerce.Domain.Entities.Category> allCategories)
        {
            var result = new List<Guid>();
            var visited = new HashSet<Guid>();
            GetDescendantsHelper(parentId, allCategories, result, visited);
            return result;
        }

        private void GetDescendantsHelper(Guid parentId, List<ECommerce.Domain.Entities.Category> allCategories, List<Guid> result, HashSet<Guid> visited)
        {
            if (visited.Contains(parentId))
                return;

            visited.Add(parentId);

            var children = allCategories.Where(c => c.ParentCategoryId == parentId).Select(c => c.Id).ToList();
            foreach (var childId in children)
            {
                result.Add(childId);
                GetDescendantsHelper(childId, allCategories, result, visited);
            }
        }
    }
}
