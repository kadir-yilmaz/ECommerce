using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions.Services
{
    public interface IFavoriteService
    {
        Task<List<FavoriteItem>> GetFavoriteItemsAsync();
        Task AddFavoriteItemAsync(string productId);
        Task RemoveFavoriteItemAsync(string favoriteItemId);
        Task RemoveFavoriteItemByProductIdAsync(string productId);
    }
}
