using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs.Account;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
namespace ViewStream.Application.Features.Account.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginCommandHandler(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var model = request.Dto;
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");
            // Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new UnauthorizedAccessException("Please confirm your email before signing in.");

            }

            if (user.IsDeleted || !user.IsActive || user.IsBlocked)
                throw new UnauthorizedAccessException("Account is disabled or blocked.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (!signInResult.Succeeded)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var jwtId = Guid.NewGuid().ToString();
            var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(user.Id, jwtId, cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName ?? string.Empty,
                    Phone = user.PhoneNumber,
                    CountryCode = user.CountryCode,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                }
            };
        }
    }
}
