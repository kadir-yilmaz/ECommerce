using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Configurations;
using ECommerce.Application.DTOs.Order;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly Iyzipay.Options _options;

        public PaymentService(IOptions<IyzipaySettings> settings)
        {
            var s = settings.Value;
            _options = new Iyzipay.Options
            {
                ApiKey = s.ApiKey,
                SecretKey = s.SecretKey,
                BaseUrl = s.BaseUrl
            };
        }

        public async Task<(bool Succeeded, string Message, string PaymentId)> ProcessPaymentAsync(
            CreateOrder order, 
            decimal totalAmount, 
            string orderCode, 
            string userEmail, 
            string userName,
            IEnumerable<PaymentBasketItem>? basketItems = null)
        {
            try
            {
                var names = userName.Split(' ');
                var firstName = names.Length > 0 ? names[0] : "Customer";
                var lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "Customer";

                // Iyzico basket items oluştur
                List<Iyzipay.Model.BasketItem> iyzicoBasketItems;

                if (basketItems != null && basketItems.Any())
                {
                    var itemList = basketItems.ToList();
                    // Oransal dağıtım: her ürünün fiyatlarının toplamı totalAmount'a eşit olmalı
                    decimal allocatedSum = 0m;
                    iyzicoBasketItems = new List<Iyzipay.Model.BasketItem>();

                    for (int i = 0; i < itemList.Count; i++)
                    {
                        decimal itemPrice;
                        if (i == itemList.Count - 1)
                        {
                            // Son ürüne kalan tüm tutarı ver (yuvarlama farkını absorbe et)
                            itemPrice = totalAmount - allocatedSum;
                        }
                        else
                        {
                            // 2 ondalık basamağa yuvarla
                            itemPrice = System.Math.Round(itemList[i].Price, 2);
                        }

                        // Iyzico sıfır veya negatif item kabul etmez, minimum 0.01
                        if (itemPrice < 0.01m)
                            itemPrice = 0.01m;

                        allocatedSum += itemPrice;

                        iyzicoBasketItems.Add(new Iyzipay.Model.BasketItem
                        {
                            Id = itemList[i].Id,
                            Name = itemList[i].Name,
                            Category1 = itemList[i].Category,
                            ItemType = BasketItemType.PHYSICAL.ToString(),
                            Price = itemPrice.ToString("F2", CultureInfo.InvariantCulture)
                        });
                    }
                }
                else
                {
                    // Fallback: tek kalem
                    iyzicoBasketItems = new List<Iyzipay.Model.BasketItem>
                    {
                        new Iyzipay.Model.BasketItem
                        {
                            Id = "BI" + orderCode,
                            Name = "E-Commerce Basket Total",
                            Category1 = "Shopping",
                            ItemType = BasketItemType.PHYSICAL.ToString(),
                            Price = totalAmount.ToString("F2", CultureInfo.InvariantCulture)
                        }
                    };
                }

                var request = new CreatePaymentRequest
                {
                    Locale = Locale.TR.ToString(),
                    ConversationId = $"ECommerce-{orderCode}",
                    Price = totalAmount.ToString("F2", CultureInfo.InvariantCulture),
                    PaidPrice = totalAmount.ToString("F2", CultureInfo.InvariantCulture),
                    Currency = Currency.TRY.ToString(),
                    Installment = 1,
                    BasketId = orderCode,
                    PaymentChannel = PaymentChannel.WEB.ToString(),
                    PaymentGroup = PaymentGroup.PRODUCT.ToString(),
                    PaymentCard = new PaymentCard
                    {
                        CardHolderName = order.CardHolderName,
                        CardNumber = order.CardNumber,
                        ExpireMonth = order.ExpireMonth,
                        ExpireYear = order.ExpireYear,
                        Cvc = order.Cvv,
                        RegisterCard = 0
                    },
                    Buyer = new Buyer
                    {
                        Id = "BY" + orderCode,
                        Name = firstName,
                        Surname = lastName,
                        GsmNumber = order.PhoneNumber,
                        Email = userEmail,
                        IdentityNumber = "74300864791", // Test identity number
                        RegistrationAddress = order.AddressLine,
                        City = order.City,
                        Country = "Turkey",
                        ZipCode = order.PostalCode,
                        Ip = "127.0.0.1"
                    },
                    ShippingAddress = new Iyzipay.Model.Address
                    {
                        ContactName = order.ContactName,
                        City = order.City,
                        Country = "Turkey",
                        Description = order.AddressLine,
                        ZipCode = order.PostalCode
                    },
                    BillingAddress = new Iyzipay.Model.Address
                    {
                        ContactName = order.ContactName,
                        City = order.City,
                        Country = "Turkey",
                        Description = order.AddressLine,
                        ZipCode = order.PostalCode
                    },
                    BasketItems = iyzicoBasketItems
                };

                var payment = await Payment.Create(request, _options);

                if (payment.Status == "success")
                {
                    return (true, "Ödeme başarılı.", payment.PaymentId);
                }
                else
                {
                    return (false, payment.ErrorMessage, null);
                }
            }
            catch (System.Exception ex)
            {
                return (false, $"Ödeme işlemi sırasında bir hata oluştu: {ex.Message}", null);
            }
        }
    }
}

