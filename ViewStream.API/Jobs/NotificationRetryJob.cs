using Hangfire;
using ViewStream.Application.Contracts;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.API.Jobs;

public class NotificationRetryJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _messageBus;

    public NotificationRetryJob(IUnitOfWork unitOfWork, IMessageBus messageBus)
    {
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task Execute()
    {
        var failedNotifications = await _unitOfWork.Notifications.GetFailedPendingAsync(5);
        foreach (var notif in failedNotifications)
        {
            notif.Status = "Pending";
            notif.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            await _messageBus.Publish(new SendNotificationMessage { NotificationId = notif.Id });
        }
    }
}