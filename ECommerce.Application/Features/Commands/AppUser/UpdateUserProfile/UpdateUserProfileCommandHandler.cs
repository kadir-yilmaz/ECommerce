using ECommerce.Application.Abstractions.Services;
using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommandRequest, UpdateUserProfileCommandResponse>
    {
        readonly IUserService _userService;

        public UpdateUserProfileCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UpdateUserProfileCommandResponse> Handle(UpdateUserProfileCommandRequest request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUserProfileAsync(request.UserId, request.UserName, request.Email, request.NameSurname);
        }
    }
}
