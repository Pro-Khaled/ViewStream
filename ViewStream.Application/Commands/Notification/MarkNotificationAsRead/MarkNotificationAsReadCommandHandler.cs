using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Notification.MarkNotificationAsRead
{
    using Notification = ViewStream.Domain.Entities.Notification;
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<MarkNotificationAsReadCommandHandler> _logger;

        public MarkNotificationAsReadCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<MarkNotificationAsReadCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Marking notification Id: {Id} as read for UserId: {UserId}", request.Id, request.UserId);

            var notification = await _unitOfWork.Notifications.GetByIdAsync<long>(request.Id, cancellationToken);
            if (notification == null || notification.UserId != request.UserId)
            {
                _logger.LogWarning("Notification not found or access denied. Id: {Id}, UserId: {UserId}", request.Id, request.UserId);
                return false;
            }

            if (notification.IsRead == true)
            {
                _logger.LogInformation("Notification already read. Id: {Id}", request.Id);
                return true;
            }

            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Notification, object>(
                tableName: "Notifications",
                recordId: notification.Id,
                action: "UPDATE",
                oldValues: new { IsRead = false },
                newValues: new { IsRead = true },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Notification marked as read. Id: {Id}", request.Id);
            return true;
        }
    }
}
