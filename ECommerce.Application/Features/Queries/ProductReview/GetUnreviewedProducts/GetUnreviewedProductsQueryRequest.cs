using MediatR;
using System.Collections.Generic;

namespace ECommerce.Application.Features.Queries.ProductReview.GetUnreviewedProducts
{
    public class GetUnreviewedProductsQueryRequest : IRequest<List<GetUnreviewedProductsQueryResponse>>
    {
    }
}
