using MediatR;

namespace ECommerce.Application.Features.Commands.Favorite.RemoveFavoriteItemByProductId
{
    public class RemoveFavoriteItemByProductIdCommandRequest : IRequest<RemoveFavoriteItemByProductIdCommandResponse>
    {
        public string ProductId { get; set; }
    }
}
