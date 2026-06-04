using ECommerce.Application.Repositories;
using ECommerce.Application.Repositories.Category;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Queries.Product.GetSearchSuggestions
{
    public class GetSearchSuggestionsQueryHandler : IRequestHandler<GetSearchSuggestionsQueryRequest, List<GetSearchSuggestionsQueryResponse>>
    {
        readonly IProductReadRepository _productReadRepository;
        readonly ICategoryReadRepository _categoryReadRepository;


        public GetSearchSuggestionsQueryHandler(IProductReadRepository productReadRepository, ICategoryReadRepository categoryReadRepository)
        {
            _productReadRepository = productReadRepository;
            _categoryReadRepository = categoryReadRepository;
        }

        public async Task<List<GetSearchSuggestionsQueryResponse>> Handle(GetSearchSuggestionsQueryRequest request, CancellationToken cancellationToken)
        {
            var results = new List<GetSearchSuggestionsQueryResponse>();
            if (string.IsNullOrWhiteSpace(request.Q))
                return results;

            var q = request.Q.Trim().ToLower();

            // 1. BRAND MATCHES (Marka)
            var matchedBrandsDb = await _productReadRepository.GetAll(false)
                .Where(p => p.Brand != null && p.Brand.ToLower().Contains(q))
                .Select(p => p.Brand)
                .Distinct()
                .ToListAsync(cancellationToken);

            var matchedBrands = matchedBrandsDb
                .OrderByDescending(brand => brand.ToLower().StartsWith(q))
                .Take(3)
                .ToList();

            foreach (var brand in matchedBrands)
            {
                if (!string.IsNullOrWhiteSpace(brand))
                {
                    results.Add(new GetSearchSuggestionsQueryResponse
                    {
                        Text = brand,
                        Type = "Marka"
                    });
                }
            }

            // 2. CATEGORY MATCHES (Kategori)
            var matchedCategories = await _categoryReadRepository.GetAll(false)
                .Where(c => c.Name.ToLower().Contains(q))
                .Select(c => new { c.Id, c.Name })
                .Take(3)
                .ToListAsync(cancellationToken);

            foreach (var cat in matchedCategories)
            {
                results.Add(new GetSearchSuggestionsQueryResponse
                {
                    Text = cat.Name,
                    Type = "Kategori",
                    TargetId = cat.Id.ToString()
                });
            }

            // 3. PRODUCT MATCHES (Ürün)
            var matchedProducts = await _productReadRepository.GetAll(false)
                .Where(p => p.Name.ToLower().Contains(q) || (p.Brand != null && p.Brand.ToLower().Contains(q)))
                .Select(p => new { p.Id, p.Name, p.Brand })
                .Take(6)
                .ToListAsync(cancellationToken);

            foreach (var prod in matchedProducts)
            {
                var displayName = string.IsNullOrWhiteSpace(prod.Brand) ? prod.Name : $"{prod.Brand} {prod.Name}";
                results.Add(new GetSearchSuggestionsQueryResponse
                {
                    Text = displayName,
                    Type = "Ürün",
                    TargetId = prod.Id.ToString()
                });
            }

            // Deduplicate + limit
            return results
                .GroupBy(r => r.Text.ToLower())
                .Select(g => g.First())
                .Take(12)
                .ToList();
        }
    }
}
