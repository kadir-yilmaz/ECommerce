using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ECommerce.Infrastructure.Services
{
    public class MailService : IMailService
    {
        readonly MailSettings _mailSettings;
        readonly IConfiguration _configuration;

        public MailService(IOptions<MailSettings> mailSettings, IConfiguration configuration)
        {
            _mailSettings = mailSettings.Value;
            _configuration = configuration;
        }

        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendMailAsync(new[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendMailAsync(string[] recipients, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new()
            {
                IsBodyHtml = isBodyHtml,
                Subject = subject,
                Body = body,
                From = new MailAddress(
                    _mailSettings.SenderEmail ?? string.Empty,
                    _mailSettings.SenderName ?? "E-Commerce",
                    Encoding.UTF8)
            };

            foreach (var to in recipients)
                mail.To.Add(to);

            using SmtpClient smtp = new()
            {
                Credentials = new NetworkCredential(_mailSettings.SenderEmail, _mailSettings.Password),
                Port = _mailSettings.SmtpPort != 0 ? _mailSettings.SmtpPort : 587,
                EnableSsl = true,
                Host = _mailSettings.SmtpServer ?? "smtp.gmail.com"
            };

            await smtp.SendMailAsync(mail);
        }

        public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
        {
            string clientUrl = _configuration["ClientUrls:PasswordReset"] ?? _configuration["AngularClientUrl"] ?? "http://localhost:4200";

            StringBuilder mail = new();
            mail.AppendLine("Merhaba,<br>Eger yeni sifre talebinde bulunduysaniz asagidaki linkten sifrenizi yenileyebilirsiniz.<br><br>");
            mail.AppendLine($"<strong><a target=\"_blank\" href=\"{clientUrl}/update-password/{userId}/{resetToken}\">Yeni sifre talebi icin tiklayiniz.</a></strong><br><br>");
            mail.AppendLine("<span style=\"font-size:12px;\">Bu talep size ait degilse bu maili dikkate almayabilirsiniz.</span><br><br>");
            mail.AppendLine("Saygilarimizla,<br>E-Commerce");

            await SendMailAsync(to, "Sifre Yenileme Talebi", mail.ToString());
        }

        public async Task SendCompletedOrderMailAsync(string to, string orderCode, DateTime orderDate, string userName)
        {
            string mail = $"Sayin {userName}, merhaba.<br>{orderDate:G} tarihinde vermis oldugunuz {orderCode} kodlu siparisiniz tamamlanmis ve kargo firmasina verilmis bulunmaktadir.<br>Keyifli kullanmanizi dileriz.";

            await SendMailAsync(to, $"{orderCode} numarali siparisiniz tamamlandi", mail);
        }

        public async Task SendEmailConfirmationMailAsync(string to, string userId, string token)
        {
            string clientUrl = _configuration["ClientUrls:EmailConfirmation"] ?? _configuration["AngularClientUrl"] ?? "http://localhost:4200";
            
            // URL encode the token to handle special characters
            string encodedToken = System.Web.HttpUtility.UrlEncode(token);

            StringBuilder mail = new();
            mail.AppendLine("Merhaba,<br>E-posta adresinizi dogrulamak icin asagidaki linke tiklayiniz.<br><br>");
            mail.AppendLine($"<strong><a target=\"_blank\" href=\"{clientUrl}/confirm-email/{userId}/{encodedToken}\">E-posta adresimi dogrula</a></strong><br><br>");
            mail.AppendLine("<span style=\"font-size:12px;\">Bu talep size ait degilse bu maili dikkate almayabilirsiniz.</span><br><br>");
            mail.AppendLine("Saygilarimizla,<br>E-Commerce");

            await SendMailAsync(to, "E-posta Adresi Dogrulama", mail.ToString());
        }

        public async Task SendEmailConfirmationCodeAsync(string to, string code)
        {
            StringBuilder mail = new();
            mail.AppendLine("Merhaba,<br>E-posta adresinizi dogrulamak icin asagidaki kodu kullaniniz.<br><br>");
            mail.AppendLine($"<div style=\"background-color: #f0f0f0; padding: 20px; text-align: center; border-radius: 8px;\">");
            mail.AppendLine($"<h2 style=\"color: #333; font-size: 32px; letter-spacing: 5px; margin: 0;\">{code}</h2>");
            mail.AppendLine($"</div><br>");
            mail.AppendLine("<p>Bu kod 10 dakika boyunca gecerlidir.</p>");
            mail.AppendLine("<span style=\"font-size:12px;\">Bu talep size ait degilse bu maili dikkate almayabilirsiniz.</span><br><br>");
            mail.AppendLine("Saygilarimizla,<br>E-Commerce");

            await SendMailAsync(to, "E-posta Dogrulama Kodu", mail.ToString());
        }
    }
}
