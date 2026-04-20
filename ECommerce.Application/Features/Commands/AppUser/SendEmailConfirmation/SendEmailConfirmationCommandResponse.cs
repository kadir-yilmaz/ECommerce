namespace ECommerce.Application.Features.Commands.AppUser.SendEmailConfirmation
{
    public class SendEmailConfirmationCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
