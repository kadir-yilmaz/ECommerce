using MediatR;

namespace ECommerce.Application.Features.Commands.Favorite.AddFavoriteItem
{
    public class AddFavoriteItemCommandRequest : IRequest<AddFavoriteItemCommandResponse>
    {
        public string ProductId { get; set; }
    }
}
