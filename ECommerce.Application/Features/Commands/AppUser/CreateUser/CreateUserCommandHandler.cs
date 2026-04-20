using ECommerce.Application.Abstractions.Services;
using ECommerce.Application.DTOs.User;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Application.Features.Commands.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        readonly IUserService _userService;
        readonly UserManager<Domain.Entities.AppUser> _userManager;

        public CreateUserCommandHandler(IUserService userService, UserManager<Domain.Entities.AppUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            // Generate unique username from email
            var emailPrefix = request.Email.Split('@')[0];
            var username = emailPrefix;
            
            // Check if username exists
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                // Add random suffix to make it unique
                username = $"{emailPrefix}_{Guid.NewGuid().ToString().Substring(0, 8)}";
            }
            
            // Use email prefix as name surname initially (user can update later)
            var nameSurname = emailPrefix;

            CreateUserResponse response = await _userService.CreateAsync(new()
            {
                Email = request.Email,
                NameSurname = nameSurname,
                Password = request.Password,
                PasswordConfirm = request.PasswordConfirm,
                Username = username,
            });

            return new()
            {
                Message = response.Message,
                Succeeded = response.Succeeded,
            };
        }
    }
}
