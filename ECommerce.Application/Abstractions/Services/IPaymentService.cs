using ECommerce.Application.DTOs.Order;
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
            string userName);
    }
}
