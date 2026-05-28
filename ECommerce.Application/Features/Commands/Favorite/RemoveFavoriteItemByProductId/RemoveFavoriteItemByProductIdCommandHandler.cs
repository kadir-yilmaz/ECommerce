using ECommerce.Application.Abstractions.Services;
using MediatR;

namespace ECommerce.Application.Features.Commands.Favorite.RemoveFavoriteItemByProductId
{
    public class RemoveFavoriteItemByProductIdCommandHandler : IRequestHandler<RemoveFavoriteItemByProductIdCommandRequest, RemoveFavoriteItemByProductIdCommandResponse>
    {
        readonly IFavoriteService _favoriteService;

        public RemoveFavoriteItemByProductIdCommandHandler(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        public async Task<RemoveFavoriteItemByProductIdCommandResponse> Handle(RemoveFavoriteItemByProductIdCommandRequest request, CancellationToken cancellationToken)
        {
            await _favoriteService.RemoveFavoriteItemByProductIdAsync(request.ProductId);
            return new();
        }
    }
}
