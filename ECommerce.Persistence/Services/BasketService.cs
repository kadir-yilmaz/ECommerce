using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Repositories;
using ECommerce.Application.ViewModels.Baskets;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Persistence.Services
{
    public class BasketService : IBasketService
    {
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly UserManager<AppUser> _userManager;
        readonly IOrderReadRepository _orderReadRepository;
        readonly IBasketWriteRepository _basketWriteRepository;
        readonly IBasketReadRepository _basketReadRepository;
        readonly IBasketItemWriteRepository _basketItemWriteRepository;
        readonly IBasketItemReadRepository _basketItemReadRepository;
        readonly IDistributedCache _distributedCache;
        readonly IProductReadRepository _productReadRepository;

        public BasketService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IOrderReadRepository orderReadRepository, IBasketWriteRepository basketWriteRepository, IBasketItemWriteRepository basketItemWriteRepository, IBasketItemReadRepository basketItemReadRepository, IBasketReadRepository basketReadRepository, IDistributedCache distributedCache, IProductReadRepository productReadRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _orderReadRepository = orderReadRepository;
            _basketWriteRepository = basketWriteRepository;
            _basketItemWriteRepository = basketItemWriteRepository;
            _basketItemReadRepository = basketItemReadRepository;
            _basketReadRepository = basketReadRepository;
            _distributedCache = distributedCache;
            _productReadRepository = productReadRepository;
        }

        private bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
        
        private string? GuestBasketId => _httpContextAccessor.HttpContext?.Request?.Headers["Basket-Id"].FirstOrDefault();

        #region Guest Basket Helpers
        private async Task<SessionBasket> GetGuestBasketAsync()
        {
            if (string.IsNullOrEmpty(GuestBasketId)) 
                return new SessionBasket() { BasketId = Guid.NewGuid().ToString() };

            var cachedBasket = await _distributedCache.GetStringAsync(GuestBasketId);
            if (string.IsNullOrEmpty(cachedBasket))
                return new SessionBasket() { BasketId = GuestBasketId };

            return JsonSerializer.Deserialize<SessionBasket>(cachedBasket) ?? new SessionBasket() { BasketId = GuestBasketId };
        }

        private async Task SaveGuestBasketAsync(SessionBasket sessionBasket)
        {
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30));
            await _distributedCache.SetStringAsync(sessionBasket.BasketId, JsonSerializer.Serialize(sessionBasket), options);
        }
        #endregion

        #region User Basket Helpers
        private async Task<Basket?> GetOrCreateUserBasketAsync()
        {
            var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                Serilog.Log.Warning("GetOrCreateUserBasketAsync: No username in context");
                return null;
            }

            AppUser? user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                Serilog.Log.Warning("GetOrCreateUserBasketAsync: User {Username} not found", username);
                return null;
            }

            Serilog.Log.Information("GetOrCreateUserBasketAsync: Looking for basket for user {UserId}", user.Id);

            // Get all baskets for this user
            var allBaskets = await _basketReadRepository.Table
                .Include(b => b.BasketItems)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

            Serilog.Log.Information("Found {Count} total baskets for user {UserId}", allBaskets.Count, user.Id);

            // Get all order IDs
            var orderIds = await _orderReadRepository.Table
                .Select(o => o.Id)
                .ToListAsync();

            Serilog.Log.Information("Found {Count} total orders", orderIds.Count);

            // Find basket without order
            var existingBasket = allBaskets.FirstOrDefault(b => !orderIds.Contains(b.Id));

            if (existingBasket != null)
            {
                Serilog.Log.Information("Found existing active basket {BasketId} for user {UserId}", existingBasket.Id, user.Id);
                return existingBasket;
            }

            // If no basket exists, create a new one
            try
            {
                Serilog.Log.Information("No active basket found, creating new basket for user {UserId}", user.Id);
                var newBasket = new Basket { UserId = user.Id };
                await _basketWriteRepository.AddAsync(newBasket);
                await _basketWriteRepository.SaveAsync();
                Serilog.Log.Information("Successfully created new basket {BasketId} for user {UserId}", newBasket.Id, user.Id);
                return newBasket;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("IX_Baskets_UserId") == true)
            {
                // If we get a duplicate key error, fetch again
                Serilog.Log.Warning("Duplicate basket creation detected for user {UserId}, fetching existing basket", user.Id);
                
                // Wait a bit and retry
                await Task.Delay(100);
                
                var allBasketsRetry = await _basketReadRepository.Table
                    .Include(b => b.BasketItems)
                    .Where(b => b.UserId == user.Id)
                    .OrderByDescending(b => b.CreatedDate)
                    .ToListAsync();

                var orderIdsRetry = await _orderReadRepository.Table
                    .Select(o => o.Id)
                    .ToListAsync();

                var retryBasket = allBasketsRetry.FirstOrDefault(b => !orderIdsRetry.Contains(b.Id));

                if (retryBasket != null)
                {
                    Serilog.Log.Information("Successfully fetched basket {BasketId} after duplicate detection", retryBasket.Id);
                    return retryBasket;
                }

                Serilog.Log.Error("Failed to fetch basket after duplicate detection for user {UserId}. Exception: {Exception}", user.Id, ex.Message);
                
                // Return null instead of throwing to prevent app crash
                return null;
            }
        }
        #endregion

        public async Task AddItemToBasketAsync(VM_Create_BasketItem basketItem)
        {
            var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            Serilog.Log.Information("AddItemToBasketAsync - IsAuthenticated: {IsAuthenticated}, Username: {Username}", IsAuthenticated, username);
            
            if (IsAuthenticated)
            {
                // DB operation for authenticated users
                Basket? basket = await GetOrCreateUserBasketAsync();
                if (basket != null)
                {
                    BasketItem? _basketItem = await _basketItemReadRepository.GetSingleAsync(bi => bi.BasketId == basket.Id && bi.ProductId == Guid.Parse(basketItem.ProductId));
                    if (_basketItem != null)
                        _basketItem.Quantity += basketItem.Quantity;
                    else
                        await _basketItemWriteRepository.AddAsync(new()
                        {
                            BasketId = basket.Id,
                            ProductId = Guid.Parse(basketItem.ProductId),
                            Quantity = basketItem.Quantity
                        });

                    await _basketItemWriteRepository.SaveAsync();
                    Serilog.Log.Information("Item added to DB basket. BasketId: {BasketId}, ProductId: {ProductId}", basket.Id, basketItem.ProductId);
                }
            }
            else
            {
                // Session operation for guest users
                Serilog.Log.Information("User not authenticated, saving to session cache");
                var sessionBasket = await GetGuestBasketAsync();
                var item = sessionBasket.BasketItems.FirstOrDefault(bi => bi.ProductId == basketItem.ProductId);
                if (item != null)
                {
                    item.Quantity += basketItem.Quantity;
                }
                else
                {
                    sessionBasket.BasketItems.Add(new SessionBasketItem
                    {
                        ProductId = basketItem.ProductId,
                        Quantity = basketItem.Quantity
                    });
                }
                await SaveGuestBasketAsync(sessionBasket);
            }
        }

        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            Serilog.Log.Information("GetBasketItemsAsync - IsAuthenticated: {IsAuthenticated}, Username: {Username}", IsAuthenticated, username);
            
            if (IsAuthenticated)
            {
                // Get from DB for authenticated users
                Basket? basket = await GetOrCreateUserBasketAsync();
                if (basket == null) {
                    Serilog.Log.Warning("No basket found or created for user {Username}", username);
                    return new List<BasketItem>();
                }
                
                Serilog.Log.Information("Fetching basket items for basket {BasketId}", basket.Id);
                
                Basket? result = await _basketReadRepository.Table
                     .Include(b => b.BasketItems)
                     .ThenInclude(bi => bi.Product)
                     .FirstOrDefaultAsync(b => b.Id == basket.Id);

                var items = result?.BasketItems.ToList() ?? new List<BasketItem>();
                Serilog.Log.Information("Found {Count} items in DB basket", items.Count);
                return items;
            }
            else
            {
                // Get from session for guest users
                var sessionBasket = await GetGuestBasketAsync();
                var basketItems = new List<BasketItem>();
                
                foreach (var sbItem in sessionBasket.BasketItems)
                {
                    var product = await _productReadRepository.GetByIdAsync(sbItem.ProductId);
                    if (product != null)
                    {
                        basketItems.Add(new BasketItem
                        {
                            Id = Guid.Parse(sbItem.BasketItemId),
                            ProductId = Guid.Parse(sbItem.ProductId),
                            Quantity = sbItem.Quantity,
                            Product = product
                        });
                    }
                }
                Serilog.Log.Information("Found {Count} items in session basket", basketItems.Count);
                return basketItems;
            }
        }

        public async Task RemoveBasketItemAsync(string basketItemId)
        {
            if (IsAuthenticated)
            {
                // DB operation for authenticated users
                BasketItem? basketItem = await _basketItemReadRepository.GetByIdAsync(basketItemId);
                if (basketItem != null)
                {
                    _basketItemWriteRepository.Remove(basketItem);
                    await _basketItemWriteRepository.SaveAsync();
                }
            }
            else
            {
                // Session operation for guest users
                var sessionBasket = await GetGuestBasketAsync();
                var item = sessionBasket.BasketItems.FirstOrDefault(bi => bi.BasketItemId == basketItemId);
                if (item != null)
                {
                    sessionBasket.BasketItems.Remove(item);
                    await SaveGuestBasketAsync(sessionBasket);
                }
            }
        }

        public async Task UpdateQuantityAsync(VM_Update_BasketItem basketItem)
        {
            if (IsAuthenticated)
            {
                // DB operation for authenticated users
                BasketItem? _basketItem = await _basketItemReadRepository.GetByIdAsync(basketItem.BasketItemId);
                if (_basketItem != null)
                {
                    _basketItem.Quantity = basketItem.Quantity;
                    await _basketItemWriteRepository.SaveAsync();
                }
            }
            else
            {
                // Session operation for guest users
                var sessionBasket = await GetGuestBasketAsync();
                var item = sessionBasket.BasketItems.FirstOrDefault(bi => bi.BasketItemId == basketItem.BasketItemId);
                if (item != null)
                {
                    item.Quantity = basketItem.Quantity;
                    await SaveGuestBasketAsync(sessionBasket);
                }
            }
        }

        public async Task<Basket?> GetUserActiveBasketAsync()
        {
            return await GetOrCreateUserBasketAsync();
        }

        public async Task MergeGuestBasketAsync()
        {
            await MergeGuestBasketAsync(GuestBasketId);
        }

        public async Task MergeGuestBasketAsync(string? guestBasketId)
        {
            var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            
            if (string.IsNullOrEmpty(username))
            {
                Serilog.Log.Warning("MergeGuestBasketAsync called but no username found in context.");
                return;
            }

            if (string.IsNullOrEmpty(guestBasketId))
            {
                Serilog.Log.Information("No guest basket ID provided. Nothing to merge.");
                return;
            }

            Serilog.Log.Information("Starting merge for user {Username}. GuestBasketId: {GuestBasketId}", username, guestBasketId);

            var cachedBasketStr = await _distributedCache.GetStringAsync(guestBasketId);
            if (string.IsNullOrEmpty(cachedBasketStr))
            {
                Serilog.Log.Information("Guest basket {GuestBasketId} not found in cache.", guestBasketId);
                return;
            }

            var sessionBasket = JsonSerializer.Deserialize<SessionBasket>(cachedBasketStr);
            if (sessionBasket == null || !sessionBasket.BasketItems.Any())
            {
                Serilog.Log.Information("Guest basket is empty. Clearing cache.");
                await _distributedCache.RemoveAsync(guestBasketId);
                return;
            }

            Serilog.Log.Information("Merging guest basket {GuestId} with {Count} items.", guestBasketId, sessionBasket.BasketItems.Count);

            // Get user by username
            AppUser? user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                Serilog.Log.Error("User {Username} not found.", username);
                return;
            }

            var activeBasket = await GetOrCreateUserBasketAsync();
            if (activeBasket == null)
            {
                Serilog.Log.Error("Failed to get or create user basket.");
                return;
            }

            foreach (var sessionItem in sessionBasket.BasketItems)
            {
                if (string.IsNullOrEmpty(sessionItem.ProductId)) continue;

                var existingItem = activeBasket.BasketItems?.FirstOrDefault(bi => bi.ProductId == Guid.Parse(sessionItem.ProductId));

                if (existingItem != null)
                {
                    existingItem.Quantity += sessionItem.Quantity;
                    _basketItemWriteRepository.Update(existingItem);
                    Serilog.Log.Information("Updated product {ProductId} quantity to {Quantity}", sessionItem.ProductId, existingItem.Quantity);
                }
                else
                {
                    await _basketItemWriteRepository.AddAsync(new BasketItem
                    {
                        BasketId = activeBasket.Id,
                        ProductId = Guid.Parse(sessionItem.ProductId),
                        Quantity = sessionItem.Quantity
                    });
                    Serilog.Log.Information("Added product {ProductId} with quantity {Quantity}", sessionItem.ProductId, sessionItem.Quantity);
                }
            }

            await _basketItemWriteRepository.SaveAsync();
            await _distributedCache.RemoveAsync(guestBasketId);

            Serilog.Log.Information("Merge completed successfully for user {Username}.", username);
        }
    }
}
