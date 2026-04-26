using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Notification.MarkAllNotificationsAsRead
{
    using Notification = ViewStream.Domain.Entities.Notification;
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<MarkAllNotificationsAsReadCommandHandler> _logger;

        public MarkAllNotificationsAsReadCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<MarkAllNotificationsAsReadCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Marking all notifications as read for UserId: {UserId}", request.UserId);

            var notifications = await _unitOfWork.Notifications.FindAsync(
                n => n.UserId == request.UserId && n.IsRead != true,
                cancellationToken: cancellationToken);

            var list = notifications.ToList();
            if (!list.Any())
            {
                _logger.LogInformation("No unread notifications found for UserId: {UserId}", request.UserId);
                return true;
            }

            foreach (var notification in list)
            {
                notification.IsRead = true;
                _unitOfWork.Notifications.Update(notification);

                _auditContext.SetAudit<Notification, object>(
                    tableName: "Notifications",
                    recordId: notification.Id,
                    action: "UPDATE",
                    oldValues: new { IsRead = false },
                    newValues: new { IsRead = true },
                    changedByUserId: request.ActorUserId
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Marked {Count} notifications as read for UserId: {UserId}", list.Count, request.UserId);
            return true;
        }
    }
}
