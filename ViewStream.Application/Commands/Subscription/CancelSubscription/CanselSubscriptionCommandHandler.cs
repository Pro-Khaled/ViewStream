using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subscription.DeleteSubscription
{
    using Subscription = ViewStream.Domain.Entities.Subscription;
    public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CancelSubscriptionCommandHandler> _logger;

        public CancelSubscriptionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CancelSubscriptionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cancelling subscription Id: {SubscriptionId}", request.Id);

            var sub = await _unitOfWork.Subscriptions.GetByIdAsync<long>(request.Id, cancellationToken);
            if (sub == null)
            {
                _logger.LogWarning("Subscription not found. Id: {SubscriptionId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<SubscriptionDto>(sub);
            sub.Status = "canceled";
            sub.EndDate = DateOnly.FromDateTime(DateTime.UtcNow);
            sub.AutoRenew = false;
            _unitOfWork.Subscriptions.Update(sub);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Subscription, object>(
                tableName: "Subscriptions",
                recordId: sub.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: new { sub.Status, sub.EndDate, sub.AutoRenew },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Subscription cancelled. Id: {SubscriptionId}", sub.Id);
            return true;
        }
    }
}
