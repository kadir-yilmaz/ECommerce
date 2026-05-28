namespace ECommerce.Application.Configurations
{
    public class MailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Password { get; set; }
        public string AdminEmail { get; set; }
    }
}
