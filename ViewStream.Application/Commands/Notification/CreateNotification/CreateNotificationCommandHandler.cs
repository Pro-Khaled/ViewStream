using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Contracts;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Notification.CreateNotification
{
    using Notification = ViewStream.Domain.Entities.Notification;
    using User = ViewStream.Domain.Entities.User;

    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, NotificationDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly UserManager<User> _userManager;
        private readonly IMessageBus _messageBus;
        private readonly IInAppNotificationSender _inAppNotificationSender;
        private readonly ILogger<CreateNotificationCommandHandler> _logger;

        public CreateNotificationCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            UserManager<User> userManager,
            IMessageBus messageBus,
            IInAppNotificationSender inAppNotificationSender,
            ILogger<CreateNotificationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _userManager = userManager;
            _messageBus = messageBus;
            _inAppNotificationSender = inAppNotificationSender;
            _logger = logger;
        }

        public async Task<NotificationDto> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // Resolve UserId from Email when Email is provided
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.UserId == 0)
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    throw new InvalidOperationException($"No user found with email '{dto.Email}'.");
                dto.UserId = user.Id;
            }

            _logger.LogInformation("Admin {AdminId} sending notification to UserId: {UserId}, Type: {Type}, Channel: {Channel}",
                request.ActorUserId, dto.UserId, dto.NotificationType, dto.Channel ?? "All");

            var notification = _mapper.Map<Notification>(dto);
            notification.IsRead = false;
            notification.CreatedAt = DateTime.UtcNow;
            notification.Status = "Pending";
            notification.RetryCount = 0;
            notification.Channel ??= "All";

            await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 1. In‑App delivery (immediate, using abstraction)
            if (notification.Channel == "InApp" || notification.Channel == "All")
            {
                await _inAppNotificationSender.SendToUserAsync(notification.UserId, new
                {
                    notification.Id,
                    notification.Title,
                    notification.Body,
                    notification.NotificationType
                }, cancellationToken);
                _logger.LogDebug("In‑app notification {Id} sent to user {UserId}", notification.Id, notification.UserId);
            }

            // 2. Email and/or Push delivery (asynchronous via RabbitMQ)
            if (notification.Channel == "Email" || notification.Channel == "Push" || notification.Channel == "All")
            {
                await _messageBus.Publish(new SendNotificationMessage { NotificationId = notification.Id });
                _logger.LogDebug("Background job published for notification {Id} (channel {Channel})", notification.Id, notification.Channel);
            }

            // Audit log
            _auditContext.SetAudit<Notification, object>(
                tableName: "Notifications",
                recordId: notification.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { notification.UserId, notification.Title, notification.NotificationType, notification.Channel },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Notification {NotificationId} sent to UserId: {UserId}",
                notification.Id, dto.UserId);

            return _mapper.Map<NotificationDto>(notification);
        }
    }
}