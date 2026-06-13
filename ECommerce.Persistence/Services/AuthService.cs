using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Abstractions.Token;
using ECommerce.Application.DTOs;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Helpers;
using ECommerce.Domain.Entities;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ECommerce.Persistence.Services
{
    public class AuthService : IAuthService
    {
        readonly UserManager<AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly SignInManager<AppUser> _signInManager;
        readonly IUserService _userService;
        readonly IMailService _mailService;
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly ECommerce.Persistence.Contexts.ECommerceDbContext _context;
        readonly IConfiguration _configuration;

        public AuthService(
            UserManager<AppUser> userManager,
            ITokenHandler tokenHandler,
            SignInManager<AppUser> signInManager,
            IUserService userService,
            IMailService mailService,
            IHttpContextAccessor httpContextAccessor,
            ECommerce.Persistence.Contexts.ECommerceDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _signInManager = signInManager;
            _userService = userService;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _configuration = configuration;
        }

        public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
        {
            // Only allow login with email
            AppUser? user = await _userManager.FindByEmailAsync(usernameOrEmail);
            if (user == null)
                user = await _userManager.FindByNameAsync(usernameOrEmail);

            if (user == null)
                throw new NotFoundUserException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime, user, roles);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 7);
                
                // Write refresh token to HttpOnly cookie (valid for 7 days)
                SetRefreshTokenCookie(token.RefreshToken, DateTime.UtcNow.AddDays(7));
                
                return token;
            }
            throw new AuthenticationErrorException();
        }
 
        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
            var userRefreshToken = await _context.UserRefreshTokens
                .Include(urt => urt.User)
                .FirstOrDefaultAsync(urt => urt.Token == refreshToken && !urt.IsRevoked && urt.ExpirationDate > DateTime.UtcNow);

            if (userRefreshToken != null)
            {
                var user = userRefreshToken.User;
                var roles = await _userManager.GetRolesAsync(user);
                Token token = _tokenHandler.CreateAccessToken(900, user, roles);
                
                // Keep the original refresh token and do not update the DB expiration date (no sliding/rotation)
                token.RefreshToken = userRefreshToken.Token;
                
                // Set the HTTP cookie using the original expiration date so it strictly expires after 7 days from initial login
                SetRefreshTokenCookie(token.RefreshToken, userRefreshToken.ExpirationDate);
                
                return token;
            }
            else
                throw new NotFoundUserException();
        }
 
        private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                httpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = expires
                });
            }
        }

        public async Task<Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["ExternalLoginSettings:Google:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");

            AppUser? user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            bool result = user != null;

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        UserName = payload.Email,
                        NameSurname = payload.Name
                    };
                    var identityResult = await _userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
                else
                {
                    result = true;
                }
            }

            if (result)
            {
                // Check if this login is already added to user, if not add it
                var logins = await _userManager.GetLoginsAsync(user);
                if (!logins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey))
                {
                    await _userManager.AddLoginAsync(user, info);
                }

                var roles = await _userManager.GetRolesAsync(user);
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime, user, roles);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 7);
                SetRefreshTokenCookie(token.RefreshToken, DateTime.UtcNow.AddDays(7));
                return token;
            }

            throw new Exception("Invalid external authentication.");
        }

        public async Task PasswordResetAsnyc(string email)
        {
            AppUser? user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                resetToken = resetToken.UrlEncode();

                await _mailService.SendPasswordResetMailAsync(email, user.Id, resetToken);
            }
        }

        public async Task<bool> VerifyResetTokenAsync(string resetToken, string userId)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                resetToken = resetToken.UrlDecode();
                return await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);
            }
            return false;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var userRefreshToken = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(urt => urt.Token == refreshToken && !urt.IsRevoked);

            if (userRefreshToken != null)
            {
                userRefreshToken.IsRevoked = true;
                userRefreshToken.RevokedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
