using MediatR;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Queries.Product.GetSearchSuggestions
{
    public class GetSearchSuggestionsQueryRequest : IRequest<List<GetSearchSuggestionsQueryResponse>>
    {
        public string Q { get; set; }
    }
}
