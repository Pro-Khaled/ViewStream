using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Commands.User.DeleteUser
{

    using User = ViewStream.Domain.Entities.User;

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(
            UserManager<User> userManager,
            IAuditContext auditContext,
            ILogger<DeleteUserCommandHandler> logger)
        {
            _userManager = userManager;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin {AdminId} soft‑deleting UserId: {UserId}", request.ActorUserId, request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return false;

            var oldValues = new { user.IsDeleted, user.IsActive };
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            _auditContext.SetAudit<User, object>(
                tableName: "Users",
                recordId: user.Id,
                action: "DELETE",
                oldValues: oldValues,
                newValues: new { user.IsDeleted, user.IsActive },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("UserId: {UserId} soft‑deleted by Admin {AdminId}", user.Id, request.ActorUserId);
            return true;
        }
    }
}
