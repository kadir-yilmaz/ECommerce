using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.DTOs.Order;
using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Persistence.Services
{
    public class OrderService : IOrderService
    {
        readonly IOrderWriteRepository _orderWriteRepository;
        readonly IOrderReadRepository _orderReadRepository;
        readonly ICompletedOrderWriteRepository _completedOrderWriteRepository;
        readonly ICompletedOrderReadRepository _completedOrderReadRepository;
        readonly IBasketService _basketService;
        readonly ILogger<OrderService> _logger;
        readonly IPaymentService _paymentService;
        readonly IMailService _mailService;
        readonly IBasketReadRepository _basketReadRepository;
        readonly UserManager<AppUser> _userManager;
        readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(
            IOrderWriteRepository orderWriteRepository, 
            IOrderReadRepository orderReadRepository, 
            ICompletedOrderWriteRepository completedOrderWriteRepository, 
            ICompletedOrderReadRepository completedOrderReadRepository, 
            IBasketService basketService, 
            ILogger<OrderService> logger,
            IPaymentService paymentService,
            IMailService mailService,
            IBasketReadRepository basketReadRepository,
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _completedOrderWriteRepository = completedOrderWriteRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
            _basketService = basketService;
            _logger = logger;
            _paymentService = paymentService;
            _mailService = mailService;
            _basketReadRepository = basketReadRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateOrderAsync(CreateOrder createOrder)
        {
            var orderCode = (new Random().NextDouble() * 10000).ToString();
            orderCode = orderCode.Substring(orderCode.IndexOf(".") + 1, orderCode.Length - orderCode.IndexOf(".") - 1);

            var basket = await _basketService.GetUserActiveBasketAsync();
            if (basket == null)
            {
                _logger.LogWarning("CreateOrderAsync failed because no active basket was found.");
                throw new InvalidOperationException("Aktif sepet bulunamadi.");
            }

            // Load complete basket details including products
            var completeBasket = await _basketReadRepository.Table
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(b => b.Id == basket.Id);

            if (completeBasket == null || completeBasket.BasketItems == null || !completeBasket.BasketItems.Any())
            {
                throw new InvalidOperationException("Sepetinizde ürün bulunamadı.");
            }

            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                throw new InvalidOperationException("Kullanıcı oturum açmamış.");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new InvalidOperationException("Kullanıcı bulunamadı.");

            // Calculate total price of basket items
            decimal totalPrice = (decimal)completeBasket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity);

            _logger.LogInformation("Processing payment on Iyzipay for order {OrderCode}. Total: {TotalPrice}", orderCode, totalPrice);

            // Call Iyzipay to process payment
            var (paymentSucceeded, paymentMessage, paymentId) = await _paymentService.ProcessPaymentAsync(
                createOrder,
                totalPrice,
                orderCode,
                user.Email ?? "test@email.com",
                user.NameSurname ?? user.UserName ?? "Customer"
            );

            if (!paymentSucceeded)
            {
                _logger.LogWarning("CreateOrderAsync: Payment failed for order {OrderCode}: {Message}", orderCode, paymentMessage);
                throw new Exception($"Ödeme başarısız: {paymentMessage}");
            }

            _logger.LogInformation("Payment successful for order {OrderCode}. PaymentId: {PaymentId}", orderCode, paymentId);

            var address = string.Join(Environment.NewLine, new[]
            {
                $"Teslimat Kisisi: {createOrder.ContactName}",
                $"Telefon: {createOrder.PhoneNumber}",
                $"Bolge: {createOrder.Neighborhood}, {createOrder.District}/{createOrder.City}",
                $"Posta Kodu: {createOrder.PostalCode}",
                $"Acik Adres: {createOrder.AddressLine}"
            });

            await _orderWriteRepository.AddAsync(new()
            {
                Address = address,
                Id = basket.Id,
                Description = createOrder.Description,
                OrderCode = orderCode
            });
            await _orderWriteRepository.SaveAsync();

            _logger.LogInformation("Order {OrderCode} saved successfully. Sending email notification...", orderCode);

            // Send order confirmation email
            try
            {
                await _mailService.SendMailAsync(
                    user.Email ?? "test@email.com",
                    $"{orderCode} Numaralı Siparişiniz Alındı",
                    $"Sayın {user.NameSurname}, merhaba.<br>{orderCode} kodlu siparişiniz başarıyla alınmıştır. Toplam Tutar: {totalPrice:C2}. Ödemeniz Iyzico güvencesiyle tahsil edilmiştir. Bizi tercih ettiğiniz için teşekkür ederiz."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order placed email for order {OrderCode}", orderCode);
            }
        }

        public async Task<ListOrder> GetAllOrdersAsync(int page, int size)
        {
            var query = _orderReadRepository.Table.Include(o => o.Basket)
                      .ThenInclude(b => b.User)
                      .Include(o => o.Basket)
                         .ThenInclude(b => b.BasketItems)
                         .ThenInclude(bi => bi.Product);

            var data = query.OrderByDescending(o => o.CreatedDate).Skip(page * size).Take(size);

            var data2 = from order in data
                        join completedOrder in _completedOrderReadRepository.Table
                           on order.Id equals completedOrder.OrderId into co
                        from _co in co.DefaultIfEmpty()
                        select new
                        {
                            Id = order.Id,
                            CreatedDate = order.CreatedDate,
                            OrderCode = order.OrderCode,
                            Basket = order.Basket,
                            Completed = _co != null ? true : false
                        };

            return new()
            {
                TotalOrderCount = await query.CountAsync(),
                Orders = await data2.Select(o => new
                {
                    Id = o.Id,
                    CreatedDate = o.CreatedDate,
                    OrderCode = o.OrderCode,
                    TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                    UserName = o.Basket.User.UserName,
                    o.Completed
                }).ToListAsync()
            };
        }

        public async Task<ListOrder> GetOrdersByUserAsync(string userId, int page, int size)
        {
            var query = _orderReadRepository.Table
                .Include(o => o.Basket)
                    .ThenInclude(b => b.User)
                .Include(o => o.Basket)
                    .ThenInclude(b => b.BasketItems)
                        .ThenInclude(bi => bi.Product)
                .Where(o => o.Basket.User.Id == userId);

            var totalCount = await query.CountAsync();
            var data = query.OrderByDescending(o => o.CreatedDate).Skip(page * size).Take(size);

            var data2 = from order in data
                        join completedOrder in _completedOrderReadRepository.Table
                           on order.Id equals completedOrder.OrderId into co
                        from _co in co.DefaultIfEmpty()
                        select new
                        {
                            Id = order.Id,
                            CreatedDate = order.CreatedDate,
                            OrderCode = order.OrderCode,
                            Basket = order.Basket,
                            Completed = _co != null ? true : false
                        };

            return new()
            {
                TotalOrderCount = totalCount,
                Orders = await data2.Select(o => new
                {
                    Id = o.Id,
                    CreatedDate = o.CreatedDate,
                    OrderCode = o.OrderCode,
                    TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                    UserName = o.Basket.User.UserName,
                    o.Completed
                }).ToListAsync()
            };
        }

        public async Task<SingleOrder> GetOrderByIdAsync(string id)
        {
            var data = _orderReadRepository.Table
                                 .Include(o => o.Basket)
                                     .ThenInclude(b => b.BasketItems)
                                         .ThenInclude(bi => bi.Product);

            var data2 = await (from order in data
                               join completedOrder in _completedOrderReadRepository.Table
                                    on order.Id equals completedOrder.OrderId into co
                               from _co in co.DefaultIfEmpty()
                               select new
                               {
                                   Id = order.Id,
                                   CreatedDate = order.CreatedDate,
                                   OrderCode = order.OrderCode,
                                   Basket = order.Basket,
                                   Completed = _co != null ? true : false,
                                   Address = order.Address,
                                   Description = order.Description
                               }).FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            return new()
            {
                Id = data2.Id.ToString(),
                BasketItems = data2.Basket.BasketItems.Select(bi => new
                {
                    bi.Product.Name,
                    bi.Product.Price,
                    bi.Quantity
                }),
                Address = data2.Address,
                CreatedDate = data2.CreatedDate,
                Description = data2.Description,
                OrderCode = data2.OrderCode,
                Completed = data2.Completed
            };
        }

        public async Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id)
        {
            Order? order = await _orderReadRepository.Table
                .Include(o => o.Basket)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            if (order != null)
            {
                await _completedOrderWriteRepository.AddAsync(new() { OrderId = Guid.Parse(id) });
                return (await _completedOrderWriteRepository.SaveAsync() > 0, new()
                {
                    OrderCode = order.OrderCode,
                    OrderDate = order.CreatedDate,
                    Username = order.Basket.User.UserName,
                    EMail = order.Basket.User.Email
                });
            }
            return (false, null);
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            Order? order = await _orderReadRepository.GetByIdAsync(id);
            if (order != null)
            {
                _orderWriteRepository.Remove(order);
                await _orderWriteRepository.SaveAsync();
                return true;
            }
            return false;
        }
    }
}
