using ECommerce.Application.Abstractions.Services;
using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.SendEmailConfirmation
{
    public class SendEmailConfirmationCommandHandler : IRequestHandler<SendEmailConfirmationCommandRequest, SendEmailConfirmationCommandResponse>
    {
        readonly IUserService _userService;

        public SendEmailConfirmationCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SendEmailConfirmationCommandResponse> Handle(SendEmailConfirmationCommandRequest request, CancellationToken cancellationToken)
        {
            return await _userService.SendEmailConfirmationAsync(request.UserId);
        }
    }
}
