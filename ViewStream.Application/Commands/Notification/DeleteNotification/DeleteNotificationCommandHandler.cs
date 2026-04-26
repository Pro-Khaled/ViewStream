using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Notification.DeleteNotification
{
    using Notification = ViewStream.Domain.Entities.Notification;
    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteNotificationCommandHandler> _logger;

        public DeleteNotificationCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<DeleteNotificationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting notification Id: {Id} for UserId: {UserId}", request.Id, request.UserId);

            var notification = await _unitOfWork.Notifications.GetByIdAsync<long>(request.Id, cancellationToken);
            if (notification == null || notification.UserId != request.UserId)
            {
                _logger.LogWarning("Notification not found or access denied. Id: {Id}, UserId: {UserId}", request.Id, request.UserId);
                return false;
            }

            var oldValues = new { notification.Title, notification.Body, notification.IsRead };
            _unitOfWork.Notifications.Delete(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Notification, object>(
                tableName: "Notifications",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Notification deleted. Id: {Id}", request.Id);
            return true;
        }
    }
}
