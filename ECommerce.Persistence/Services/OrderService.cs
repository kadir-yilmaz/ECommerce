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
        readonly ECommerce.Application.Abstractions.Discount.IDiscountService _discountService;
        readonly ECommerce.Application.Abstractions.Discount.IRewardService _rewardService;
        readonly ECommerce.Application.Repositories.DiscountCoupon.IDiscountCouponReadRepository _discountCouponReadRepository;
        readonly ECommerce.Application.Repositories.DiscountCoupon.IDiscountCouponWriteRepository _discountCouponWriteRepository;
        readonly ECommerce.Application.Repositories.UserCoupon.IUserCouponReadRepository _userCouponReadRepository;
        readonly ECommerce.Application.Repositories.UserCoupon.IUserCouponWriteRepository _userCouponWriteRepository;
        readonly ECommerce.Application.Repositories.OrderDiscount.IOrderDiscountWriteRepository _orderDiscountWriteRepository;

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
            IHttpContextAccessor httpContextAccessor,
            ECommerce.Application.Abstractions.Discount.IDiscountService discountService,
            ECommerce.Application.Abstractions.Discount.IRewardService rewardService,
            ECommerce.Application.Repositories.DiscountCoupon.IDiscountCouponReadRepository discountCouponReadRepository,
            ECommerce.Application.Repositories.DiscountCoupon.IDiscountCouponWriteRepository discountCouponWriteRepository,
            ECommerce.Application.Repositories.UserCoupon.IUserCouponReadRepository userCouponReadRepository,
            ECommerce.Application.Repositories.UserCoupon.IUserCouponWriteRepository userCouponWriteRepository,
            ECommerce.Application.Repositories.OrderDiscount.IOrderDiscountWriteRepository orderDiscountWriteRepository)
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
            _discountService = discountService;
            _rewardService = rewardService;
            _discountCouponReadRepository = discountCouponReadRepository;
            _discountCouponWriteRepository = discountCouponWriteRepository;
            _userCouponReadRepository = userCouponReadRepository;
            _userCouponWriteRepository = userCouponWriteRepository;
            _orderDiscountWriteRepository = orderDiscountWriteRepository;
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

            // Load complete basket details including products and their categories
            var completeBasket = await _basketReadRepository.Table
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.Product)
                .ThenInclude(p => p.Category)
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
            decimal basePrice = (decimal)completeBasket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity);

            // Apply discounts
            var discountRequest = new ECommerce.Application.DTOs.Discount.CalculateDiscountRequest
            {
                CouponCode = createOrder.CouponCode,
                UserId = user.Id,
                Items = completeBasket.BasketItems.Select(bi => new ECommerce.Application.DTOs.Discount.CalculateDiscountItem
                {
                    ProductId = bi.ProductId.ToString(),
                    ProductName = bi.Product.Name,
                    CategoryId = bi.Product.CategoryId.ToString(),
                    Brand = bi.Product.Brand,
                    Quantity = bi.Quantity,
                    UnitPrice = (decimal)bi.Product.Price
                }).ToList()
            };

            var discountResponse = await _discountService.CalculateDiscountAsync(discountRequest);
            
            decimal finalTotalPrice = discountResponse != null 
                ? (decimal)discountResponse.FinalTotal 
                : basePrice;

            if (discountResponse != null)
            {
                finalTotalPrice += discountResponse.ShippingFee;
            }
            else
            {
                finalTotalPrice += 50m;
            }

            if (finalTotalPrice > IyzipayMaxPaymentAmount)
            {
                _logger.LogWarning("CreateOrderAsync blocked because order total {TotalPrice} exceeds Iyzipay max amount {MaxAmount}.", finalTotalPrice, IyzipayMaxPaymentAmount);
                return (false, "Iyzico 100.000 TL uzeri odemeleri kabul etmez. Lutfen sepet tutarini 100.000 TL veya altina dusurun.");
            }

            _logger.LogInformation("Processing payment on Iyzipay for order {OrderCode}. Total: {TotalPrice}", orderCode, finalTotalPrice);

            // Iyzico'ya ürün bazlı basket items oluştur (indirim oransal dağıtımı)
            var paymentBasketItems = BuildPaymentBasketItems(completeBasket.BasketItems, discountResponse, finalTotalPrice, orderCode);

            // Call Iyzipay to process payment
            var (paymentSucceeded, paymentMessage, paymentId) = await _paymentService.ProcessPaymentAsync(
                createOrder,
                finalTotalPrice,
                orderCode,
                user.Email ?? "test@email.com",
                user.NameSurname ?? user.UserName ?? "Customer",
                paymentBasketItems
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
                Description = string.Empty,
                OrderCode = orderCode,
                Status = 1 // Ödeme tamamlandı
            });
            await _orderWriteRepository.SaveAsync();

            // OrderDiscount kayıtlarını oluştur
            if (discountResponse?.AppliedDiscounts?.Any() == true)
            {
                foreach (var discount in discountResponse.AppliedDiscounts)
                {
                    await _orderDiscountWriteRepository.AddAsync(new ECommerce.Domain.Entities.OrderDiscount
                    {
                        OrderId = basket.Id,
                        DiscountName = discount.DiscountName,
                        DiscountType = discount.DiscountType,
                        DiscountAmount = discount.DiscountAmount
                    });
                }
                await _orderDiscountWriteRepository.SaveAsync();
            }

            // Check and grant rewards immediately after placing order
            try
            {
                await _rewardService.CheckAndGrantRewardsAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking rewards for user {UserId} after order {OrderCode} created", user.Id, orderCode);
            }

            // Decrement stock for each ordered product
            foreach (var basketItem in completeBasket.BasketItems)
            {
                if (basketItem.Product != null)
                    basketItem.Product.Stock = Math.Max(0, basketItem.Product.Stock - basketItem.Quantity);
            }
            await _productWriteRepository.SaveAsync();

            // Mark Coupon as Used
            if (!string.IsNullOrEmpty(createOrder.CouponCode))
            {
                var appliedCoupon = await _discountCouponReadRepository.GetWhere(c => c.Code == createOrder.CouponCode).FirstOrDefaultAsync();
                if (appliedCoupon != null)
                {
                    appliedCoupon.UsedCount++;
                    _discountCouponWriteRepository.Update(appliedCoupon);

                    var userCoupon = await _userCouponReadRepository.GetWhere(uc => uc.DiscountCouponId == appliedCoupon.Id && uc.UserId == user.Id).FirstOrDefaultAsync();
                    if (userCoupon != null)
                    {
                        userCoupon.IsUsed = true;
                        userCoupon.UsedDate = DateTime.UtcNow;
                        _userCouponWriteRepository.Update(userCoupon);
                    }
                    else
                    {
                        await _userCouponWriteRepository.AddAsync(new UserCoupon
                        {
                            DiscountCouponId = appliedCoupon.Id,
                            UserId = user.Id,
                            IsUsed = true,
                            UsedDate = DateTime.UtcNow
                        });
                    }
                    await _userCouponWriteRepository.SaveAsync();
                    await _discountCouponWriteRepository.SaveAsync();
                }
            }

            _logger.LogInformation("Order {OrderCode} saved successfully. Sending email notification...", orderCode);

            try
            {
                var discountHtml = string.Empty;
                if (discountResponse?.AppliedDiscounts?.Any() == true)
                {
                    var discountRows = string.Join("", discountResponse.AppliedDiscounts.Select(d =>
                        $"<tr><td style='padding:4px 8px;color:#555;'>{d.DiscountName} ({d.DiscountType})</td><td style='padding:4px 8px;color:#e53935;text-align:right;'>-{d.DiscountAmount:C2}</td></tr>"
                    ));
                    discountHtml = $"<br><br><strong>Uygulanan İndirimler:</strong><table style='width:100%;border-collapse:collapse;margin-top:6px;'>{discountRows}<tr style='border-top:1px solid #ddd;'><td style='padding:4px 8px;font-weight:bold;'>Toplam İndirim</td><td style='padding:4px 8px;color:#e53935;text-align:right;font-weight:bold;'>-{discountResponse.TotalDiscount:C2}</td></tr></table>";
                }

                await _mailService.SendMailAsync(
                    user.Email ?? "test@email.com",
                    $"{orderCode} Numaralı Siparişiniz Alındı",
                    $"Sayın {user.NameSurname}, merhaba.<br>{orderCode} kodlu siparişiniz başarıyla alınmıştır.<br><br>" +
                    $"<strong>Ürün Toplamı:</strong> {discountResponse?.OriginalTotal ?? basePrice:C2}<br>" +
                    $"Kargo: {(discountResponse?.IsShippingFree == true ? "Ücretsiz" : $"{discountResponse?.ShippingFee ?? 50m:C2}")}<br>" +
                    $"{discountHtml}<br>" +
                    $"<strong>Ödenecek Tutar: {finalTotalPrice:C2}</strong><br><br>" +
                    $"Ödemeniz Iyzico güvencesiyle tahsil edilmiştir. Bizi tercih ettiğiniz için teşekkür ederiz."
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
            var query = _orderReadRepository.Table
                      .Include(o => o.Basket)
                          .ThenInclude(b => b.User)
                      .Include(o => o.Basket)
                          .ThenInclude(b => b.BasketItems)
                          .ThenInclude(bi => bi.Product)
                      .Include(o => o.OrderDiscounts);

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
                            OrderDiscounts = order.OrderDiscounts,
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
                    BasePrice = (decimal)o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                    TotalDiscount = o.OrderDiscounts.Sum(d => d.DiscountAmount),
                    TotalPrice = (decimal)o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity) - o.OrderDiscounts.Sum(d => d.DiscountAmount),
                    OrderItems = o.Basket.BasketItems.Select(bi => new
                    {
                        ProductName = bi.Product.Name,
                        UnitPrice = bi.Product.Price,
                        Quantity = bi.Quantity,
                        TotalPrice = bi.Product.Price * bi.Quantity
                    }),
                    OrderDiscounts = o.OrderDiscounts.Select(d => new
                    {
                        d.DiscountName,
                        d.DiscountType,
                        d.DiscountAmount
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
                .Include(o => o.OrderDiscounts)
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
                            OrderDiscounts = order.OrderDiscounts,
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
                    BasePrice = (decimal)o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                    TotalDiscount = o.OrderDiscounts.Sum(d => d.DiscountAmount),
                    TotalPrice = (decimal)o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity) - o.OrderDiscounts.Sum(d => d.DiscountAmount),
                    OrderItems = o.Basket.BasketItems.Select(bi => new
                    {
                        ProductId = bi.ProductId.ToString(),
                        ProductName = bi.Product.Name,
                        UnitPrice = bi.Product.Price,
                        Quantity = bi.Quantity,
                        TotalPrice = bi.Product.Price * bi.Quantity
                    }).ToList(),
                    OrderDiscounts = o.OrderDiscounts.Select(d => new
                    {
                        d.DiscountName,
                        d.DiscountType,
                        d.DiscountAmount
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
                                         .ThenInclude(bi => bi.Product)
                                 .Include(o => o.OrderDiscounts);

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
                                   OrderDiscounts = order.OrderDiscounts,
                                   Completed = _co != null ? true : false,
                                   Address = order.Address,
                                   Description = order.Description,
                                   Status = order.Status,
                                   CargoCompany = order.CargoCompany,
                                   TrackingNumber = order.TrackingNumber
                               }).FirstOrDefaultAsync(o => o.Id == orderId);

            var basePrice = data2.Basket.BasketItems.Sum(bi => (decimal)bi.Product.Price * bi.Quantity);
            var totalDiscount = data2.OrderDiscounts?.Sum(d => d.DiscountAmount) ?? 0m;

            return new()
            {
                Id = data2.Id.ToString(),
                BasketItems = data2.Basket.BasketItems.Select(bi => new
                {
                    ProductId = bi.ProductId.ToString(),
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
                TrackingNumber = data2.TrackingNumber,
                BasePrice = basePrice,
                TotalDiscount = totalDiscount,
                TotalPrice = basePrice - totalDiscount,
                OrderDiscounts = data2.OrderDiscounts?.Select(d => new SingleOrderDiscount
                {
                    DiscountName = d.DiscountName,
                    DiscountType = d.DiscountType,
                    DiscountAmount = d.DiscountAmount
                }).ToList() ?? new()
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
            Order? order = await _orderReadRepository.Table
                .Include(o => o.Basket)
                .FirstOrDefaultAsync(o => o.Id == Guid.Parse(orderId));
            if (order == null)
                return false;

            order.Status = status;
            _orderWriteRepository.Update(order);
            await _orderWriteRepository.SaveAsync();

            _logger.LogInformation("Order {OrderId} status updated to {Status}", orderId, status);

            // Sipariş Delivered (5) statüsüne geçtiyse ödül kontrolü yap
            if (status == 5 && order.Basket != null)
            {
                try
                {
                    await _rewardService.CheckAndGrantRewardsAsync(order.Basket.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking rewards for user {UserId} after order {OrderId} delivered", order.Basket.UserId, orderId);
                }
            }

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

        /// <summary>
        /// Iyzico'ya gönderilecek ürün bazlı basket items listesini oluşturur.
        /// Toplam indirim tutarı ürünlere ağırlıklarına göre oransal dağıtılır.
        /// Kargo ücreti son ürüne eklenir.
        /// </summary>
        private static List<ECommerce.Application.Abstractions.Services.PaymentBasketItem> BuildPaymentBasketItems(
            IEnumerable<ECommerce.Domain.Entities.BasketItem> basketItems,
            ECommerce.Application.DTOs.Discount.CalculateDiscountResponse? discountResponse,
            decimal finalTotalPrice,
            string orderCode)
        {
            var items = basketItems.ToList();
            if (!items.Any())
            {
                return new List<ECommerce.Application.Abstractions.Services.PaymentBasketItem>
                {
                    new() { Id = "BI" + orderCode, Name = "Basket Total", Category = "Shopping", Price = finalTotalPrice }
                };
            }

            decimal originalTotal = items.Sum(bi => (decimal)bi.Product.Price * bi.Quantity);
            decimal totalDiscount = discountResponse?.TotalDiscount ?? 0m;
            decimal shippingFee = discountResponse?.ShippingFee ?? 50m;

            var result = new List<ECommerce.Application.Abstractions.Services.PaymentBasketItem>();

            for (int i = 0; i < items.Count; i++)
            {
                var bi = items[i];
                decimal itemBase = (decimal)bi.Product.Price * bi.Quantity;

                // Her ürüne oransal indirim dağıt
                decimal itemDiscount = originalTotal > 0
                    ? Math.Round(totalDiscount * (itemBase / originalTotal), 2)
                    : 0m;

                decimal itemNetPrice = itemBase - itemDiscount;

                // Son ürüne kargo ücretini ekle
                if (i == items.Count - 1)
                    itemNetPrice += shippingFee;

                if (itemNetPrice < 0.01m)
                    itemNetPrice = 0.01m;

                result.Add(new ECommerce.Application.Abstractions.Services.PaymentBasketItem
                {
                    Id = $"BI{orderCode}-{i}",
                    Name = bi.Product.Name.Length > 60 ? bi.Product.Name[..60] : bi.Product.Name,
                    Category = bi.Product.Category?.Name ?? "Genel",
                    Price = itemNetPrice
                });
            }

            return result;
        }
    }
}

