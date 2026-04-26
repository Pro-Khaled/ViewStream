using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Commands.UserRole.RemoveRoleFromUser
{
    using Role = ViewStream.Domain.Entities.Role;
    using User = ViewStream.Domain.Entities.User;
    public class RemoveRoleFromUserCommandHandler : IRequestHandler<RemoveRoleFromUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RemoveRoleFromUserCommandHandler> _logger;

        public RemoveRoleFromUserCommandHandler(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IAuditContext auditContext,
            ILogger<RemoveRoleFromUserCommandHandler> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removing RoleId: {RoleId} from UserId: {UserId} by AdminUserId: {AdminUserId}",
                request.RoleId, request.UserId, request.ActorUserId);

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
            if (user == null || role == null)
            {
                _logger.LogWarning("User or Role not found. UserId: {UserId}, RoleId: {RoleId}", request.UserId, request.RoleId);
                return false;
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            if (!result.Succeeded)
                return false;

            _auditContext.SetAudit<object, object>(
                tableName: "UserRoles",
                recordId: user.Id.GetHashCode() ^ role.Id,
                action: "DELETE",
                oldValues: new { UserId = user.Id, RoleId = role.Id, RoleName = role.Name },
                newValues: null,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Role {RoleName} removed from User {UserId}", role.Name, user.Id);
            return true;
        }
    }
}
