using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Shared.Options;
using User = ViewStream.Domain.Entities.User;

namespace ViewStream.Application.Features.Account.Commands.Register
{

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly AppOptions _appOptions;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            UserManager<User> userManager,
            IEmailService emailService,
            IOptions<AppOptions> appOptions,
            IAuditContext auditContext,
            ILogger<RegisterCommandHandler> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _appOptions = appOptions.Value;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var model = request.Dto;

            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            // Case 1: Already confirmed
            if (existingUser != null && existingUser.EmailConfirmed)
            {
                _logger.LogWarning("Registration attempt with already confirmed email: {Email}", model.Email);
                return new RegisterResult(false, new[] { "Email is already registered." });
            }

            // Case 2: Email exists but not confirmed → resend confirmation
            if (existingUser != null && !existingUser.EmailConfirmed)
            {
                _logger.LogInformation("Resending confirmation email for {Email}", model.Email);
                // Update profile fields
                existingUser.FullName = model.FullName;
                existingUser.PhoneNumber = model.Phone;
                existingUser.CountryCode = model.CountryCode;
                existingUser.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(existingUser);

                await SendConfirmationEmailAsync(existingUser);
                return new RegisterResult(true, Array.Empty<string>(), RequiresEmailConfirmation: true, ConfirmationResent: true);
            }

            // Case 3: New user
            _logger.LogInformation("Creating new user: {Email}", model.Email);
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.Phone,
                CountryCode = model.CountryCode,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false,
                IsBlocked = false,
                EmailConfirmed = false
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                _logger.LogWarning("User creation failed for {Email}: {Errors}", model.Email,
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return new RegisterResult(false, createResult.Errors.Select(e => e.Description));
            }

            await _userManager.AddToRoleAsync(user, "User");
            await SendConfirmationEmailAsync(user);

            _auditContext.SetAudit<object, object>(
                "Users", user.Id, "INSERT",
                newValues: new { user.Email, user.FullName, user.IsActive },
                changedByUserId: null
            );

            _logger.LogInformation("User {UserId} registered successfully", user.Id);
            return new RegisterResult(true, Array.Empty<string>(), RequiresEmailConfirmation: true);
        }

        private async Task SendConfirmationEmailAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var confirmationLink = $"{_appOptions.BaseUrl.TrimEnd('/')}/api/account/confirm-email?userId={user.Id}&token={encodedToken}";
            await _emailService.SendEmailVerificationAsync(user.Email, user.FullName ?? user.Email, confirmationLink);
        }
    }
}
