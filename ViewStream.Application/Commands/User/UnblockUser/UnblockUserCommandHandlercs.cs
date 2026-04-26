using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Commands.User.UnblockUser
{
    using User = ViewStream.Domain.Entities.User;

    public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UnblockUserCommandHandler> _logger;

        public UnblockUserCommandHandler(
            UserManager<User> userManager,
            IAuditContext auditContext,
            ILogger<UnblockUserCommandHandler> logger)
        {
            _userManager = userManager;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UnblockUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin {AdminId} unblocking UserId: {UserId}", request.ActorUserId, request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted) return false;

            var oldValues = new { user.IsBlocked, user.BlockedReason, user.BlockedUntil };
            user.IsBlocked = false;
            user.BlockedReason = null;
            user.BlockedUntil = null;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            _auditContext.SetAudit<User, object>(
                tableName: "Users",
                recordId: user.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { user.IsBlocked, user.BlockedReason, user.BlockedUntil },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("UserId: {UserId} unblocked by Admin {AdminId}", user.Id, request.ActorUserId);
            return true;
        }
    }
}
