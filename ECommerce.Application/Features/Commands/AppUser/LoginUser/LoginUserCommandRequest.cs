using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.LoginUser
{
    public class LoginUserCommandRequest : IRequest<LoginUserCommandResponse>
    {
        public string UserNameOrEmail { get; set; } = string.Empty; // Keep same property name for compatibility
        public string Password { get; set; } = string.Empty;
    }
}
