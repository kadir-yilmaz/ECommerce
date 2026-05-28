using ECommerce.Application.Abstractions.Services;
using MediatR;

namespace ECommerce.Application.Features.Commands.Favorite.RemoveFavoriteItem
{
    public class RemoveFavoriteItemCommandHandler : IRequestHandler<RemoveFavoriteItemCommandRequest, RemoveFavoriteItemCommandResponse>
    {
        readonly IFavoriteService _favoriteService;

        public RemoveFavoriteItemCommandHandler(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        public async Task<RemoveFavoriteItemCommandResponse> Handle(RemoveFavoriteItemCommandRequest request, CancellationToken cancellationToken)
        {
            await _favoriteService.RemoveFavoriteItemAsync(request.FavoriteItemId);
            return new();
        }
    }
}
