using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Notification.CreateNotification
{
    using Notification = ViewStream.Domain.Entities.Notification;
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, NotificationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateNotificationCommandHandler> _logger;

        public CreateNotificationCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateNotificationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<NotificationDto> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin {AdminId} sending notification to UserId: {UserId}, Type: {Type}",
                request.ActorUserId, request.Dto.UserId, request.Dto.NotificationType);

            var notification = _mapper.Map<Notification>(request.Dto);
            notification.IsRead = false;
            notification.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log
            _auditContext.SetAudit<Notification, object>(
                tableName: "Notifications",
                recordId: notification.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { notification.UserId, notification.Title, notification.NotificationType },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Notification {NotificationId} sent to UserId: {UserId}",
                notification.Id, request.Dto.UserId);

            return _mapper.Map<NotificationDto>(notification);
        }
    }
}
