namespace ECommerce.Application.Features.Commands.AppUser.ConfirmEmail
{
    public class ConfirmEmailCommandResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
