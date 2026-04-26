using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Commands.User.ChangePassword
{
    using User = ViewStream.Domain.Entities.User;

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IdentityResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(
            UserManager<User> userManager,
            IAuditContext auditContext,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<IdentityResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Changing password for UserId: {UserId}", request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var result = await _userManager.ChangePasswordAsync(user, request.Dto.CurrentPassword, request.Dto.NewPassword);

            if (result.Succeeded)
            {
                _auditContext.SetAudit<User, object>(
                    tableName: "Users",
                    recordId: user.Id,
                    action: "UPDATE",
                    oldValues: new { action = "PasswordChanged" },
                    newValues: new { action = "PasswordChanged" },
                    changedByUserId: request.ActorUserId
                );
                _logger.LogInformation("Password changed for UserId: {UserId}", user.Id);
            }
            else
            {
                _logger.LogWarning("Password change failed for UserId: {UserId}", user.Id);
            }

            return result;
        }
    }
}
