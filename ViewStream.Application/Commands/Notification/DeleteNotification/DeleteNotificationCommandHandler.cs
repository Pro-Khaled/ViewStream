using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Notification.DeleteNotification
{
    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNotificationCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<bool> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync<long>(request.Id, cancellationToken);
            if (notification == null || notification.UserId != request.UserId) return false;

            _unitOfWork.Notifications.Delete(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
