using ECommerce.Application.DTOs.User;
using ECommerce.Application.Features.Queries.AppUser.GetUserProfile;
using ECommerce.Application.Features.Commands.AppUser.UpdateUserProfile;
using ECommerce.Application.Features.Commands.AppUser.SendEmailConfirmation;
using ECommerce.Application.Features.Commands.AppUser.ConfirmEmail;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateAsync(CreateUser model);
        Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate);
        Task UpdatePasswordAsync(string userId, string resetToken, string newPassword);
        Task<List<ListUser>> GetAllUsersAsync(int page, int size);
        int TotalUsersCount { get; }
        Task AssignRoleToUserAsync(string userId, string[] roles);
        Task<string[]> GetRolesToUserAsync(string userIdOrName);
        Task<bool> HasRolePermissionToEndpointAsync(string name, string code);
        Task<List<string>> GetAuthorizedMenusAsync(string username);
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<GetUserProfileQueryResponse> GetUserProfileAsync(string userId);
        Task<UpdateUserProfileCommandResponse> UpdateUserProfileAsync(string userId, string userName, string email, string nameSurname);
        Task<SendEmailConfirmationCommandResponse> SendEmailConfirmationAsync(string userId);
        Task<ConfirmEmailCommandResponse> ConfirmEmailAsync(string userId, string code);
    }
}
