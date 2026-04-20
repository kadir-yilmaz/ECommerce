
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions.Token
{
    public interface ITokenHandler
    {
        DTOs.Token CreateAccessToken(int second, AppUser appUser, IList<string> roles);
        string CreateRefreshToken();
    }
}
