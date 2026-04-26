using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;

namespace ViewStream.Application.Commands.User.UpdateProfile
{
    using User = ViewStream.Domain.Entities.User;

    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdateProfileCommandHandler> _logger;

        public UpdateProfileCommandHandler(
            UserManager<User> userManager,
            IAuditContext auditContext,
            ILogger<UpdateProfileCommandHandler> logger)
        {
            _userManager = userManager;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating profile for UserId: {UserId}", request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("User not found or deleted. Id: {UserId}", request.UserId);
                return false;
            }

            var oldValues = new { user.FullName, user.PhoneNumber, user.CountryCode };
            user.FullName = request.Dto.FullName;
            user.PhoneNumber = request.Dto.PhoneNumber;
            user.CountryCode = request.Dto.CountryCode;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            _auditContext.SetAudit<User, object>(
                tableName: "Users",
                recordId: user.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { user.FullName, user.PhoneNumber, user.CountryCode },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Profile updated for UserId: {UserId}", user.Id);
            return true;
        }
    }
}
