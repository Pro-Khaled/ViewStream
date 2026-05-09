using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Shared.Options;
using User = ViewStream.Domain.Entities.User;

namespace ViewStream.Application.Features.Account.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly AppOptions _appOptions;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            UserManager<User> userManager,
            IEmailService emailService,
            IOptions<AppOptions> appOptions,
            ILogger<ForgotPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _appOptions = appOptions.Value;
            _logger = logger;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Forgot password request for email: {Email}", request.Email);

            // Always return true to avoid leaking whether an email exists
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || user.IsDeleted || !user.IsActive)
            {
                _logger.LogWarning("Forgot password: user not found or inactive for email {Email}", request.Email);
                return true; // Don't reveal that the user doesn't exist
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var resetLink = $"{_appOptions.BaseUrl.TrimEnd('/')}/#/reset-password?userId={user.Id}&token={encodedToken}";

            await _emailService.SendPasswordResetEmailAsync(
                user.Email!,
                user.FullName ?? user.Email!,
                resetLink);

            _logger.LogInformation("Password reset email sent for UserId: {UserId}", user.Id);
            return true;
        }
    }
}
