using MediatR;
using ViewStream.Application.Common;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subscription.DeleteSubscription
{
    public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CancelSubscriptionCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<bool> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var sub = await _unitOfWork.Subscriptions.GetByIdAsync<long>(request.Id, cancellationToken);
            if (sub == null) return false;
            sub.Status = "canceled";
            sub.EndDate = DateOnly.FromDateTime(DateTime.UtcNow);
            sub.AutoRenew = false;
            _unitOfWork.Subscriptions.Update(sub);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
