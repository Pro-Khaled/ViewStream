using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs.Account;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
namespace ViewStream.Application.Features.Account.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService,
            IAuditContext auditContext,
            ILogger<LoginCommandHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var model = request.Dto;
            _logger.LogInformation("Login attempt for {Email}", model.Email);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new UnauthorizedAccessException("Please confirm your email before signing in.");

            if (user.IsDeleted || !user.IsActive || user.IsBlocked)
                throw new UnauthorizedAccessException("Account is disabled or blocked.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
                throw new UnauthorizedAccessException("Invalid email or password.");

            _logger.LogInformation("User {UserId} logged in", user.Id);

            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);
            var jwtId = Guid.NewGuid().ToString();
            var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(user.Id, jwtId, cancellationToken);

            // Audit the successful login
            _auditContext.SetAudit<object, object>(
                "Users", user.Id, "LOGIN",
                newValues: new { user.Id, user.Email }, changedByUserId: user.Id
            );

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
