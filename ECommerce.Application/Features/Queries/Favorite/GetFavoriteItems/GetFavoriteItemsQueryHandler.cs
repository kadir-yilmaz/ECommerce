using ECommerce.Application.Abstractions.Services;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Queries.Favorite.GetFavoriteItems
{
    public class GetFavoriteItemsQueryHandler : IRequestHandler<GetFavoriteItemsQueryRequest, List<GetFavoriteItemsQueryResponse>>
    {
        readonly IFavoriteService _favoriteService;

        public GetFavoriteItemsQueryHandler(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        public async Task<List<GetFavoriteItemsQueryResponse>> Handle(GetFavoriteItemsQueryRequest request, CancellationToken cancellationToken)
        {
            List<FavoriteItem> favoriteItems = await _favoriteService.GetFavoriteItemsAsync();

            return favoriteItems.Select(fi => new GetFavoriteItemsQueryResponse
            {
                FavoriteItemId = fi.Id.ToString(),
                ProductId = fi.ProductId.ToString(),
                Name = fi.Product.Name,
                Price = fi.Product.Price,
                Stock = fi.Product.Stock,
                ImagePath = fi.Product.ProductImageFiles?.FirstOrDefault()?.Path
            }).ToList();
        }
    }
}
