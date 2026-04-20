using ECommerce.Application.DTOs;

namespace ECommerce.Application.Abstractions.Services.Authentications
{
    public interface IInternalAuthentication
    {
        Task<ECommerce.Application.DTOs.Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime);
        Task<ECommerce.Application.DTOs.Token> RefreshTokenLoginAsync(string refreshToken);
    }
}
