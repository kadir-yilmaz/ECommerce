using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.UpdateUserProfile
{
    public class UpdateUserProfileCommandRequest : IRequest<UpdateUserProfileCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NameSurname { get; set; } = string.Empty;
    }
}
