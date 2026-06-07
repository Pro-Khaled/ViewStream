using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Contracts;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.VideoProcessor.Consumers
{
    public class NotificationConsumer : IConsumer<SendNotificationMessage>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IPushNotificationService _pushService;
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IPushNotificationService pushService,
            ILogger<NotificationConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _pushService = pushService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendNotificationMessage> context)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(context.Message.NotificationId);
            if (notification == null)
            {
                _logger.LogError("Notification {Id} not found", context.Message.NotificationId);
                return;
            }

            try
            {
                if (notification.Channel == "Email" || notification.Channel == "All")
                    await SendEmailAsync(notification);

                if (notification.Channel == "Push" || notification.Channel == "All")
                    await SendPushAsync(notification);

                notification.Status = "Sent";
                notification.SentAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification {Id}", notification.Id);
                notification.Status = "Failed";
                notification.ErrorMessage = ex.Message;
                notification.RetryCount++;
            }
            finally
            {
                notification.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task SendEmailAsync(Notification notification)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(notification.UserId);
            if (user == null) return;
            // Assuming IEmailService has SendEmailAsync(string to, string subject, string body)
            await _emailService.SendEmailAsync(user.Email!, notification.Title, notification.Body);
        }

        private async Task SendPushAsync(Notification notification)
        {
            var tokens = await _unitOfWork.PushTokens.GetByUserIdAsync(notification.UserId);
            foreach (var token in tokens)
            {
                await _pushService.SendAsync(token.Token, notification.Title, notification.Body);
            }
        }
    }
}
