using ECommerce.Application.ViewModels.Baskets;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions.Services
{
    public interface IBasketService
    {
        public Task<List<BasketItem>> GetBasketItemsAsync();
        public Task AddItemToBasketAsync(VM_Create_BasketItem basketItem);
        public Task UpdateQuantityAsync(VM_Update_BasketItem basketItem);
        public Task RemoveBasketItemAsync(string basketItemId);
        public Task<Basket?> GetUserActiveBasketAsync();
        public Task MergeGuestBasketAsync();
        public Task MergeGuestBasketAsync(string guestBasketId);
    }
}
