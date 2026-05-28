using MediatR;

namespace ECommerce.Application.Features.Queries.Favorite.GetFavoriteItems
{
    public class GetFavoriteItemsQueryRequest : IRequest<List<GetFavoriteItemsQueryResponse>>
    {
    }
}
