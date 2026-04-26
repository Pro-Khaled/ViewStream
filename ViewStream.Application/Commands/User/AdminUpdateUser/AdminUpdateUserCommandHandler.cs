using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Commands.User.AdminUpdateUser
{
    using User = ViewStream.Domain.Entities.User;

    public class AdminUpdateUserCommandHandler : IRequestHandler<AdminUpdateUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<AdminUpdateUserCommandHandler> _logger;

        public AdminUpdateUserCommandHandler(
            UserManager<User> userManager,
            IAuditContext auditContext,
            ILogger<AdminUpdateUserCommandHandler> logger)
        {
            _userManager = userManager;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(AdminUpdateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin {AdminId} updating UserId: {UserId}", request.ActorUserId, request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted)
                return false;

            var oldValues = new { user.FullName, user.PhoneNumber, user.IsActive };
            if (request.Dto.FullName != null) user.FullName = request.Dto.FullName;
            if (request.Dto.PhoneNumber != null) user.PhoneNumber = request.Dto.PhoneNumber;
            if (request.Dto.IsActive.HasValue) user.IsActive = request.Dto.IsActive.Value;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            if (request.Dto.Roles != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, request.Dto.Roles);
            }

            _auditContext.SetAudit<User, object>(
                tableName: "Users",
                recordId: user.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { user.FullName, user.PhoneNumber, user.IsActive },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Admin {AdminId} updated UserId: {UserId}", request.ActorUserId, user.Id);
            return true;
        }
    }
}
