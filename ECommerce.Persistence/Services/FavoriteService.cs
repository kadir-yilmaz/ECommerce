using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Services
{
    public class FavoriteService : IFavoriteService
    {
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly UserManager<AppUser> _userManager;
        readonly IFavoriteReadRepository _favoriteReadRepository;
        readonly IFavoriteWriteRepository _favoriteWriteRepository;
        readonly IFavoriteItemReadRepository _favoriteItemReadRepository;
        readonly IFavoriteItemWriteRepository _favoriteItemWriteRepository;

        public FavoriteService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            IFavoriteReadRepository favoriteReadRepository,
            IFavoriteWriteRepository favoriteWriteRepository,
            IFavoriteItemReadRepository favoriteItemReadRepository,
            IFavoriteItemWriteRepository favoriteItemWriteRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _favoriteReadRepository = favoriteReadRepository;
            _favoriteWriteRepository = favoriteWriteRepository;
            _favoriteItemReadRepository = favoriteItemReadRepository;
            _favoriteItemWriteRepository = favoriteItemWriteRepository;
        }

        private async Task<Favorite> GetOrCreateUserFavoriteAsync()
        {
            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new UnauthorizedAccessException("Kullanıcı girişi gereklidir.");

            AppUser? user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

            Favorite? favorite = await _favoriteReadRepository.Table
                .Include(f => f.FavoriteItems)
                .FirstOrDefaultAsync(f => f.UserId == user.Id);

            if (favorite != null)
                return favorite;

            favorite = new Favorite { UserId = user.Id };
            await _favoriteWriteRepository.AddAsync(favorite);
            await _favoriteWriteRepository.SaveAsync();

            return favorite;
        }

        public async Task AddFavoriteItemAsync(string productId)
        {
            Favorite favorite = await GetOrCreateUserFavoriteAsync();

            var parsedProductId = Guid.Parse(productId);
            FavoriteItem? existingItem = await _favoriteItemReadRepository.GetSingleAsync(
                fi => fi.FavoriteId == favorite.Id && fi.ProductId == parsedProductId);

            if (existingItem == null)
            {
                await _favoriteItemWriteRepository.AddAsync(new FavoriteItem
                {
                    FavoriteId = favorite.Id,
                    ProductId = parsedProductId
                });
                await _favoriteItemWriteRepository.SaveAsync();
            }
        }

        public async Task<List<FavoriteItem>> GetFavoriteItemsAsync()
        {
            Favorite favorite = await GetOrCreateUserFavoriteAsync();

            Favorite? result = await _favoriteReadRepository.Table
                .Include(f => f.FavoriteItems)
                .ThenInclude(fi => fi.Product)
                .ThenInclude(p => p.ProductImageFiles)
                .FirstOrDefaultAsync(f => f.Id == favorite.Id);

            return result?.FavoriteItems.ToList() ?? new List<FavoriteItem>();
        }

        public async Task RemoveFavoriteItemAsync(string favoriteItemId)
        {
            FavoriteItem? favoriteItem = await _favoriteItemReadRepository.GetByIdAsync(favoriteItemId);
            if (favoriteItem != null)
            {
                _favoriteItemWriteRepository.Remove(favoriteItem);
                await _favoriteItemWriteRepository.SaveAsync();
            }
        }

        public async Task RemoveFavoriteItemByProductIdAsync(string productId)
        {
            Favorite favorite = await GetOrCreateUserFavoriteAsync();

            FavoriteItem? favoriteItem = await _favoriteItemReadRepository.GetSingleAsync(
                fi => fi.FavoriteId == favorite.Id && fi.ProductId == Guid.Parse(productId));

            if (favoriteItem != null)
            {
                _favoriteItemWriteRepository.Remove(favoriteItem);
                await _favoriteItemWriteRepository.SaveAsync();
            }
        }
    }
}
