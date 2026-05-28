using ECommerce.Application.Abstractions.Services;
using MediatR;

namespace ECommerce.Application.Features.Commands.Favorite.AddFavoriteItem
{
    public class AddFavoriteItemCommandHandler : IRequestHandler<AddFavoriteItemCommandRequest, AddFavoriteItemCommandResponse>
    {
        readonly IFavoriteService _favoriteService;

        public AddFavoriteItemCommandHandler(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        public async Task<AddFavoriteItemCommandResponse> Handle(AddFavoriteItemCommandRequest request, CancellationToken cancellationToken)
        {
            await _favoriteService.AddFavoriteItemAsync(request.ProductId);
            return new();
        }
    }
}
