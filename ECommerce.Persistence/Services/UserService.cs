using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.Repositories;
using ECommerce.Application.DTOs.User;
using ECommerce.Application.Helpers;
using ECommerce.Application.Features.Queries.AppUser.GetUserProfile;
using ECommerce.Application.Features.Commands.AppUser.UpdateUserProfile;
using ECommerce.Application.Features.Commands.AppUser.SendEmailConfirmation;
using ECommerce.Application.Features.Commands.AppUser.ConfirmEmail;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<AppUser> _userManager;
        readonly IEndpointReadRepository _endpointReadRepository;
        readonly IMailService _mailService;

        public UserService(UserManager<AppUser> userManager, IEndpointReadRepository endpointReadRepository, IMailService mailService)
        {
            _userManager = userManager;
            _endpointReadRepository = endpointReadRepository;
            _mailService = mailService;
        }

        public async Task<CreateUserResponse> CreateAsync(CreateUser model)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Email = model.Email,
                NameSurname = model.NameSurname,
            }, model.Password);

            CreateUserResponse response = new() { Succeeded = result.Succeeded };

            if (result.Succeeded)
            {
                response.Message = "Kullanici basariyla olusturulmustur.";
                await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(model.Username), "User");
            }
            else
                foreach (var error in result.Errors)
                    response.Message += $"{error.Code} - {error.Description}\n";

            return response;
        }

        public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = accessTokenDate.AddDays(addOnAccessTokenDate);
                await _userManager.UpdateAsync(user);
            }
            else
                throw new Exception("Kullanici bulunamadi.");
        }

        public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                resetToken = resetToken.UrlDecode();
                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                if (result.Succeeded)
                    await _userManager.UpdateSecurityStampAsync(user);
                else
                    throw new Exception("Sifre degistirme basarisiz.");
            }
        }

        public async Task<List<ListUser>> GetAllUsersAsync(int page, int size)
        {
            var users = await _userManager.Users
                  .OrderBy(u => u.UserName)
                  .Skip(page * size)
                  .Take(size)
                  .ToListAsync();

            var listUsers = new List<ListUser>();
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                listUsers.Add(new ListUser
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    NameSurname = user.NameSurname,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    UserName = user.UserName ?? string.Empty,
                    Roles = userRoles.ToArray()
                });
            }
            return listUsers;
        }

        public int TotalUsersCount => _userManager.Users.Count();

        public async Task AssignRoleToUserAsync(string userId, string[] roles)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);
                await _userManager.AddToRolesAsync(user, roles);
            }
        }

        public async Task<string[]> GetRolesToUserAsync(string userIdOrName)
        {
            AppUser? user = await _userManager.FindByIdAsync(userIdOrName);
            if (user == null)
                user = await _userManager.FindByNameAsync(userIdOrName);

            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                return userRoles.ToArray();
            }
            return Array.Empty<string>();
        }

        public async Task<bool> HasRolePermissionToEndpointAsync(string name, string code)
        {
            var userRoles = await GetRolesToUserAsync(name);
            if (!userRoles.Any())
                return false;

            if (userRoles.Contains("Admin"))
                return true;

            Endpoint? endpoint = await _endpointReadRepository.Table
                     .Include(e => e.Roles)
                     .FirstOrDefaultAsync(e => e.Code == code);

            if (endpoint == null)
                return false;

            var endpointRoles = endpoint.Roles.Select(r => r.Name);

            foreach (var userRole in userRoles)
            {
                foreach (var endpointRole in endpointRoles)
                    if (userRole == endpointRole)
                        return true;
            }

            return false;
        }
        public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                IdentityResult result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                return result.Succeeded;
            }
            return false;
        }

        public async Task<List<string>> GetAuthorizedMenusAsync(string username)
        {
            Console.WriteLine($"🔍 GetAuthorizedMenusAsync called for user: {username}");
            
            var userRoles = await GetRolesToUserAsync(username);
            Console.WriteLine($"📋 User roles: {string.Join(", ", userRoles)}");
            
            if (!userRoles.Any())
            {
                Console.WriteLine("❌ No roles found for user");
                return new List<string>();
            }

            // Admin rolü varsa tüm menülere erişim
            if (userRoles.Contains("Admin"))
            {
                Console.WriteLine("✅ Admin role detected - returning all menus");
                return new List<string> { "Products", "Orders", "Customers", "Roles", "Users" };
            }

            // Kullanıcının yetkili olduğu endpoint'leri al
            var menuNames = await _endpointReadRepository.Table
                .Include(e => e.Roles)
                .Include(e => e.Menu)
                .Where(e => e.Roles.Any(r => userRoles.Contains(r.Name)))
                .Select(e => e.Menu.Name)
                .Distinct()
                .ToListAsync();

            Console.WriteLine($"📂 Authorized menus: {string.Join(", ", menuNames)}");
            return menuNames;
        }

        public async Task<GetUserProfileQueryResponse> GetUserProfileAsync(string userId)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            return new GetUserProfileQueryResponse
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                NameSurname = user.NameSurname,
                EmailConfirmed = user.EmailConfirmed
            };
        }

        public async Task<UpdateUserProfileCommandResponse> UpdateUserProfileAsync(string userId, string userName, string email, string nameSurname)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UpdateUserProfileCommandResponse { Succeeded = false, Message = "Kullanıcı bulunamadı." };

            // Check if username is already taken by another user
            if (user.UserName != userName)
            {
                var existingUser = await _userManager.FindByNameAsync(userName);
                if (existingUser != null && existingUser.Id != userId)
                    return new UpdateUserProfileCommandResponse { Succeeded = false, Message = "Bu kullanıcı adı zaten kullanılıyor." };
            }

            // Check if email is already taken by another user
            if (user.Email != email)
            {
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null && existingUser.Id != userId)
                    return new UpdateUserProfileCommandResponse { Succeeded = false, Message = "Bu e-posta adresi zaten kullanılıyor." };
                
                // Email değiştiğinde EmailConfirmed'ı false yap
                user.EmailConfirmed = false;
            }

            user.UserName = userName;
            user.Email = email;
            user.NameSurname = nameSurname;

            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded)
                return new UpdateUserProfileCommandResponse { Succeeded = true, Message = "Profil başarıyla güncellendi." };
            
            return new UpdateUserProfileCommandResponse 
            { 
                Succeeded = false, 
                Message = string.Join(", ", result.Errors.Select(e => e.Description)) 
            };
        }

        public async Task<SendEmailConfirmationCommandResponse> SendEmailConfirmationAsync(string userId)
        {
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new SendEmailConfirmationCommandResponse { Succeeded = false, Message = "Kullanıcı bulunamadı." };

            if (user.EmailConfirmed)
                return new SendEmailConfirmationCommandResponse { Succeeded = false, Message = "E-posta zaten doğrulanmış." };

            if (string.IsNullOrEmpty(user.Email))
                return new SendEmailConfirmationCommandResponse { Succeeded = false, Message = "Kullanıcının e-posta adresi bulunamadı." };

            // Generate 6-digit code
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();
            
            // Store code in SecurityStamp temporarily (or use a separate table/cache)
            user.SecurityStamp = code;
            await _userManager.UpdateAsync(user);
            
            try
            {
                await _mailService.SendEmailConfirmationCodeAsync(user.Email, code);
                return new SendEmailConfirmationCommandResponse 
                { 
                    Succeeded = true, 
                    Message = "Doğrulama kodu e-posta adresinize gönderildi. Lütfen e-posta kutunuzu kontrol edin." 
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email sending error: {ex.Message}");
                return new SendEmailConfirmationCommandResponse 
                { 
                    Succeeded = false, 
                    Message = "E-posta gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyin." 
                };
            }
        }

        public async Task<ConfirmEmailCommandResponse> ConfirmEmailAsync(string userId, string code)
        {
            Console.WriteLine($"🔍 ConfirmEmailAsync - UserId: {userId}");
            Console.WriteLine($"🔍 ConfirmEmailAsync - Code: {code}");
            
            AppUser? user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                Console.WriteLine($"❌ User not found: {userId}");
                return new ConfirmEmailCommandResponse { Succeeded = false, Message = "Kullanıcı bulunamadı." };
            }

            Console.WriteLine($"✅ User found: {user.UserName}, EmailConfirmed: {user.EmailConfirmed}");

            if (user.EmailConfirmed)
            {
                Console.WriteLine($"⚠️ Email already confirmed for user: {user.UserName}");
                return new ConfirmEmailCommandResponse { Succeeded = false, Message = "E-posta zaten doğrulanmış." };
            }

            // Check if code matches
            if (user.SecurityStamp != code)
            {
                Console.WriteLine($"❌ Invalid code for user: {user.UserName}");
                return new ConfirmEmailCommandResponse 
                { 
                    Succeeded = false, 
                    Message = "Doğrulama kodu geçersiz." 
                };
            }

            // Confirm email
            user.EmailConfirmed = true;
            user.SecurityStamp = Guid.NewGuid().ToString(); // Reset security stamp
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                Console.WriteLine($"✅ Email confirmed successfully for user: {user.UserName}");
                return new ConfirmEmailCommandResponse 
                { 
                    Succeeded = true, 
                    Message = "E-posta adresiniz başarıyla doğrulandı." 
                };
            }

            Console.WriteLine($"❌ Email confirmation failed for user: {user.UserName}");
            return new ConfirmEmailCommandResponse 
            { 
                Succeeded = false, 
                Message = "E-posta doğrulama başarısız." 
            };
        }
    }
}
