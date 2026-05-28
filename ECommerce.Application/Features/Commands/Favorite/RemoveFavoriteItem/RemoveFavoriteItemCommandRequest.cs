using MediatR;

namespace ECommerce.Application.Features.Commands.Favorite.RemoveFavoriteItem
{
    public class RemoveFavoriteItemCommandRequest : IRequest<RemoveFavoriteItemCommandResponse>
    {
        public string FavoriteItemId { get; set; }
    }
}
