using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.User.RestoreUser
{
    using User = ViewStream.Domain.Entities.User;

    /// <summary>
    /// Handles restoring a soft-deleted user account for admin.
    /// </summary>
    public class RestoreUserCommandHandler : IRequestHandler<RestoreUserCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RestoreUserCommandHandler> _logger;

        public RestoreUserCommandHandler(
            UserManager<User> userManager,
            IAuditContext auditContext,
            ILogger<RestoreUserCommandHandler> logger)
        {
            _userManager = userManager;
            _auditContext = auditContext;
            _logger = logger;
        }

        /// <summary>
        /// Restores the user account by setting:
        /// - IsDeleted = false
        /// - DeletedAt = null
        /// - IsActive = true
        /// - UpdatedAt = UtcNow
        /// </summary>
        /// <param name="request">Restore request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if restored; otherwise false.</returns>
        public async Task<bool> Handle(RestoreUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Restoring UserId: {UserId} by admin user: {AdminUserId}", request.Id, request.RestoredByUserId);

            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
            {
                _logger.LogWarning("User not found. UserId: {UserId}", request.Id);
                return false;
            }

            if (user.IsDeleted != true)
            {
                _logger.LogWarning("User is not deleted. UserId: {UserId}", request.Id);
                return false;
            }

            var oldValues = new
            {
                user.IsDeleted,
                user.DeletedAt,
                user.IsActive
            };

            user.IsDeleted = false;
            user.DeletedAt = null;
            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to restore user. UserId: {UserId}. Errors: {Errors}",
                    request.Id,
                    string.Join("; ", result.Errors ?? Array.Empty<IdentityError>()));
                return false;
            }

            _auditContext.SetAudit<User, object>(
                tableName: "Users",
                recordId: user.Id,
                action: "RESTORE",
                oldValues: oldValues,
                newValues: new { user.IsDeleted, user.DeletedAt, user.IsActive },
                changedByUserId: request.RestoredByUserId);

            _logger.LogInformation("User restored successfully. UserId: {UserId}", user.Id);
            return true;
        }
    }
}
