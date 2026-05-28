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
        const decimal IyzipayMaxPaymentAmount = 100000m;

        readonly IOrderWriteRepository _orderWriteRepository;
        readonly IOrderReadRepository _orderReadRepository;
        readonly ICompletedOrderWriteRepository _completedOrderWriteRepository;
        readonly ICompletedOrderReadRepository _completedOrderReadRepository;
        readonly IBasketService _basketService;
        readonly ILogger<OrderService> _logger;
        readonly IPaymentService _paymentService;
        readonly IMailService _mailService;
        readonly IBasketReadRepository _basketReadRepository;
        readonly IProductWriteRepository _productWriteRepository;
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
            IProductWriteRepository productWriteRepository,
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
            _productWriteRepository = productWriteRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool succeeded, string errorMessage)> CreateOrderAsync(CreateOrder createOrder)
        {
            var orderCode = (new Random().NextDouble() * 10000).ToString();
            orderCode = orderCode.Substring(orderCode.IndexOf(".") + 1, orderCode.Length - orderCode.IndexOf(".") - 1);

            var basket = await _basketService.GetUserActiveBasketAsync();
            if (basket == null)
            {
                _logger.LogWarning("CreateOrderAsync failed because no active basket was found.");
                return (false, "Aktif sepet bulunamadı.");
            }

            // Load complete basket details including products
            var completeBasket = await _basketReadRepository.Table
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(b => b.Id == basket.Id);

            if (completeBasket == null || completeBasket.BasketItems == null || !completeBasket.BasketItems.Any())
                return (false, "Sepetinizde ürün bulunamadı.");

            var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return (false, "Kullanıcı oturum açmamış.");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return (false, "Kullanıcı bulunamadı.");

            // Calculate total price of basket items
            decimal totalPrice = (decimal)completeBasket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity);

            if (totalPrice > IyzipayMaxPaymentAmount)
            {
                _logger.LogWarning("CreateOrderAsync blocked because order total {TotalPrice} exceeds Iyzipay max amount {MaxAmount}.", totalPrice, IyzipayMaxPaymentAmount);
                return (false, "Iyzico 100.000 TL uzeri odemeleri kabul etmez. Lutfen sepet tutarini 100.000 TL veya altina dusurun.");
            }

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
                return (false, $"Ödeme başarısız: {paymentMessage}");
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
                OrderCode = orderCode,
                Status = 1 // Ödeme tamamlandı
            });
            await _orderWriteRepository.SaveAsync();

            // Decrement stock for each ordered product
            foreach (var basketItem in completeBasket.BasketItems)
            {
                if (basketItem.Product != null)
                    basketItem.Product.Stock = Math.Max(0, basketItem.Product.Stock - basketItem.Quantity);
            }
            await _productWriteRepository.SaveAsync();

            _logger.LogInformation("Order {OrderCode} saved successfully. Sending email notification...", orderCode);

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

            return (true, string.Empty);
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
                            Completed = _co != null ? true : false,
                            Status = order.Status,
                            CargoCompany = order.CargoCompany,
                            TrackingNumber = order.TrackingNumber
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
                    OrderItems = o.Basket.BasketItems.Select(bi => new
                    {
                        ProductName = bi.Product.Name,
                        UnitPrice = bi.Product.Price,
                        Quantity = bi.Quantity,
                        TotalPrice = bi.Product.Price * bi.Quantity
                    }),
                    UserName = o.Basket.User.UserName,
                    o.Completed,
                    o.Status,
                    o.CargoCompany,
                    o.TrackingNumber
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
                .Where(o => o.Basket.UserId == userId);

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
                            Completed = _co != null ? true : false,
                            Status = order.Status,
                            CargoCompany = order.CargoCompany,
                            TrackingNumber = order.TrackingNumber
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
                    OrderItems = o.Basket.BasketItems.Select(bi => new
                    {
                        ProductName = bi.Product.Name,
                        UnitPrice = bi.Product.Price,
                        Quantity = bi.Quantity,
                        TotalPrice = bi.Product.Price * bi.Quantity
                    }).ToList(),
                    UserName = o.Basket.User.UserName,
                    o.Completed,
                    o.Status,
                    o.CargoCompany,
                    o.TrackingNumber
                }).ToListAsync()
            };
        }

        public async Task<SingleOrder> GetOrderByIdAsync(string id)
        {
            Guid orderId = Guid.Parse(id);
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
                                   Description = order.Description,
                                   Status = order.Status,
                                   CargoCompany = order.CargoCompany,
                                   TrackingNumber = order.TrackingNumber
                               }).FirstOrDefaultAsync(o => o.Id == orderId);

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
                Completed = data2.Completed,
                Status = data2.Status,
                CargoCompany = data2.CargoCompany,
                TrackingNumber = data2.TrackingNumber
            };
        }

        public async Task<(bool, CompletedOrderDTO)> CompleteOrderAsync(string id)
        {
            Guid orderId = Guid.Parse(id);
            Order? order = await _orderReadRepository.Table
                .Include(o => o.Basket)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order != null)
            {
                await _completedOrderWriteRepository.AddAsync(new() { OrderId = orderId });
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

        public async Task<bool> UpdateOrderStatusAsync(string orderId, int status)
        {
            Order? order = await _orderReadRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;

            order.Status = status;
            _orderWriteRepository.Update(order);
            await _orderWriteRepository.SaveAsync();

            _logger.LogInformation("Order {OrderId} status updated to {Status}", orderId, status);
            return true;
        }

        public async Task<(bool succeeded, CompletedOrderDTO? orderInfo)> ShipOrderAsync(string orderId, string cargoCompany, string trackingNumber)
        {
            Guid parsedOrderId = Guid.Parse(orderId);
            Order? order = await _orderReadRepository.Table
                .Include(o => o.Basket)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(o => o.Id == parsedOrderId);

            if (order == null)
                return (false, null);

            order.Status = 4; // Shipped
            order.CargoCompany = cargoCompany;
            order.TrackingNumber = trackingNumber;
            _orderWriteRepository.Update(order);
            bool succeeded = await _orderWriteRepository.SaveAsync() > 0;

            _logger.LogInformation("Order {OrderId} shipped via {CargoCompany}, tracking: {TrackingNumber}", orderId, cargoCompany, trackingNumber);

            CompletedOrderDTO orderInfo = new()
            {
                OrderCode = order.OrderCode,
                OrderDate = order.CreatedDate,
                Username = order.Basket?.User?.UserName ?? "Değerli Müşterimiz",
                EMail = order.Basket?.User?.Email ?? string.Empty
            };

            return (succeeded, orderInfo);
        }
    }
}
