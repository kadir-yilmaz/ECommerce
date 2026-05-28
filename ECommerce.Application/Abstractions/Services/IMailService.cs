namespace ECommerce.Application.Abstractions.Services
{
    public interface IMailService
    {
        Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true);
        Task SendMailAsync(string[] recipients, string subject, string body, bool isBodyHtml = true);
        Task SendPasswordResetMailAsync(string to, string userId, string resetToken);
        Task SendCompletedOrderMailAsync(string to, string orderCode, DateTime orderDate, string userName);
        Task SendShippedOrderMailAsync(string to, string orderCode, DateTime orderDate, string userName, string cargoCompany, string trackingNumber);
        Task SendEmailConfirmationMailAsync(string to, string userId, string token);
        Task SendEmailConfirmationCodeAsync(string to, string code);
    }
}
