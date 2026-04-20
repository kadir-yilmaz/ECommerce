using ECommerce.Application.Abstractions.Services;
using MediatR;

namespace ECommerce.Application.Features.Queries.AppUser.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQueryRequest, GetUserProfileQueryResponse>
    {
        readonly IUserService _userService;

        public GetUserProfileQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetUserProfileQueryResponse> Handle(GetUserProfileQueryRequest request, CancellationToken cancellationToken)
        {
            return await _userService.GetUserProfileAsync(request.UserId);
        }
    }
}
