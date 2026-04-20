using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.SendEmailConfirmation
{
    public class SendEmailConfirmationCommandRequest : IRequest<SendEmailConfirmationCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
