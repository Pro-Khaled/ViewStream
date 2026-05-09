using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Domain.Interfaces;
using User = ViewStream.Domain.Entities.User;

namespace ViewStream.Application.Features.Account.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            UserManager<User> userManager,
            IUnitOfWork unitOfWork,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResetPasswordResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            if (dto.NewPassword != dto.ConfirmPassword)
                return new ResetPasswordResult(false, new[] { "Passwords do not match." });

            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null || user.IsDeleted)
                return new ResetPasswordResult(false, new[] { "Invalid reset link." });

            _logger.LogInformation("Resetting password for UserId: {UserId}", user.Id);

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Password reset failed for UserId: {UserId}: {Errors}", user.Id,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return new ResetPasswordResult(false, result.Errors.Select(e => e.Description));
            }

            // Revoke all active refresh tokens so existing sessions are invalidated
            var activeTokens = await _unitOfWork.RefreshTokens.FindAsync(
                rt => rt.UserId == user.Id && !rt.IsRevoked,
                asNoTracking: false,
                cancellationToken: cancellationToken);

            foreach (var rt in activeTokens)
                rt.IsRevoked = true;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Password reset successful for UserId: {UserId}. All refresh tokens revoked.", user.Id);
            return new ResetPasswordResult(true, Array.Empty<string>());
        }
    }
}
