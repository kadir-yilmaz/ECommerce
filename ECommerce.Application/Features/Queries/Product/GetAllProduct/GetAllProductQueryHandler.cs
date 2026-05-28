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
        readonly ILogger<GetAllProductQueryHandler> _logger;

        public GetAllProductQueryHandler(
            IProductReadRepository productReadRepository, 
            ICategoryReadRepository categoryReadRepository,
            ILogger<GetAllProductQueryHandler> logger)
        {
            _productReadRepository = productReadRepository;
            _categoryReadRepository = categoryReadRepository;
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
                query = query.Where(p => p.Name.ToLower().Contains(search));
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

            var products = query
                .Skip(request.Page * request.Size).Take(request.Size)
                .Include(p => p.ProductImageFiles)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Stock,
                    p.Price,
                    p.CreatedDate,
                    p.UpdatedDate,
                    p.ProductImageFiles,
                    p.CategoryId
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
