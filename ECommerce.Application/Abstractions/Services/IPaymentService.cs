using ECommerce.Application.DTOs.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Application.Abstractions.Services
{
    public interface IPaymentService
    {
        Task<(bool Succeeded, string Message, string PaymentId)> ProcessPaymentAsync(
            CreateOrder order, 
            decimal totalAmount, 
            string orderCode, 
            string userEmail, 
            string userName,
            IEnumerable<PaymentBasketItem>? basketItems = null);
    }

    public class PaymentBasketItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        /// <summary>
        /// İndirim dağıtımı sonrası bu kaleme atanan tutar
        /// </summary>
        public decimal Price { get; set; }
    }
}

