using ECommerce.Application.DTOs;

namespace ECommerce.Application.Abstractions.Services.Authentications
{
    public interface IExternalAuthentication
    {
        Task<ECommerce.Application.DTOs.Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime);
        Task<ECommerce.Application.DTOs.Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime);
    }
}
