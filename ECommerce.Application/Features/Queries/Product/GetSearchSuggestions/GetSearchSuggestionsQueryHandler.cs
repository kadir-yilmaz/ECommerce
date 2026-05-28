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
            // Ürün adının ilk kelimesi marka kabul edilir.
            // "cor" → DB'de adı "cor" içeren ürünlerin ilk kelimesi → "Corsair" gibi
            var brandCandidateNames = await _productReadRepository.GetAll(false)
                .Where(p => p.Name.ToLower().Contains(q))
                .Select(p => p.Name)
                .ToListAsync(cancellationToken);

            var matchedBrands = brandCandidateNames
                .Select(name => name.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0])
                .Where(brand => !string.IsNullOrWhiteSpace(brand))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                // StartsWith önce, Contains sonra
                .OrderByDescending(brand => brand.ToLower().StartsWith(q))
                .Take(3)
                .ToList();

            foreach (var brand in matchedBrands)
            {
                results.Add(new GetSearchSuggestionsQueryResponse
                {
                    Text = brand,
                    Type = "Marka"
                });
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
                .Where(p => p.Name.ToLower().Contains(q))
                .Select(p => new { p.Id, p.Name })
                .Take(6)
                .ToListAsync(cancellationToken);

            foreach (var prod in matchedProducts)
            {
                results.Add(new GetSearchSuggestionsQueryResponse
                {
                    Text = prod.Name,
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
