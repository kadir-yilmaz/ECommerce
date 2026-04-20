using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.ConfirmEmail
{
    public class ConfirmEmailCommandRequest : IRequest<ConfirmEmailCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
