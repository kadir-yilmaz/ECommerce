using ECommerce.Application.Abstractions.Services;
using MediatR;

namespace ECommerce.Application.Features.Commands.AppUser.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommandRequest, ChangePasswordCommandResponse>
    {
        readonly IUserService _userService;

        public ChangePasswordCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ChangePasswordCommandResponse> Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            if (!request.NewPassword.Equals(request.NewPasswordConfirm))
                return new() { Succeeded = false, Message = "Lütfen şifreyi doğrulayınız." };

            bool result = await _userService.ChangePasswordAsync(request.UserId, request.OldPassword, request.NewPassword);
            
            return new() 
            { 
                Succeeded = result, 
                Message = result ? "Şifreniz başarıyla güncellenmiştir." : "Şifre güncellenirken bir hata oluştu. Lütfen eski şifrenizi kontrol ediniz." 
            };
        }
    }
}
