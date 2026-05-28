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

        private static readonly List<string> PopularBrands = new()
        {
            "Corsair", "Apple", "Dell", "Logitech", "L'Oreal", "Maybelline", "Nike", "Adidas", "Puma", "Samsung", "HP", "Asus", "Lenovo", "Beko", "Vestel"
        };

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
            var matchedBrands = PopularBrands
                .Where(b => b.ToLower().StartsWith(q) || b.ToLower().Contains(q))
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
                .Select(c => new { c.Id, c.Name, c.ParentCategoryId })
                .Take(5)
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

            // 4. PRODUCT MATCHES (Ürün)
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

            // Deduplicate by Text and prioritize exact matches or certain types
            return results
                .GroupBy(r => r.Text.ToLower())
                .Select(g => g.First())
                .Take(12)
                .ToList();
        }
    }
}
