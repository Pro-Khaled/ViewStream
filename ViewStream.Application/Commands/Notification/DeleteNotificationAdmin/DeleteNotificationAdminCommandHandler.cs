using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Notification.DeleteNotificationAdmin
{
    using Notification = ViewStream.Domain.Entities.Notification;
    public class DeleteNotificationAdminCommandHandler : IRequestHandler<DeleteNotificationAdminCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteNotificationAdminCommandHandler> _logger;

        public DeleteNotificationAdminCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<DeleteNotificationAdminCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteNotificationAdminCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin deleting notification Id: {Id} by AdminUserId: {AdminUserId}", request.Id, request.AdminUserId);

            var notification = await _unitOfWork.Notifications.GetByIdAsync<long>(request.Id, cancellationToken);
            if (notification == null)
            {
                _logger.LogWarning("Notification not found. Id: {Id}", request.Id);
                return false;
            }

            var oldValues = new { notification.Title, notification.Body, notification.IsRead };
            _unitOfWork.Notifications.Delete(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Notification, object>(
                tableName: "Notifications",
                recordId: request.Id,
                action: "DELETE_BY_ADMIN",
                oldValues: oldValues,
                changedByUserId: request.AdminUserId
            );

            _logger.LogInformation("Notification deleted by admin. Id: {Id}", request.Id);
            return true;
        }
    }
}
