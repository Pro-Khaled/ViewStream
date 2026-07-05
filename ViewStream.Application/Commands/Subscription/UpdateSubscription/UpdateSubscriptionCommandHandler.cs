using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;


namespace ViewStream.Application.Commands.Subscription.UpdateSubscription
{
    using Subscription = ViewStream.Domain.Entities.Subscription;
    public class UpdateSubscriptionCommandHandler : IRequestHandler<UpdateSubscriptionCommand, SubscriptionDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly IStripeService _stripeService;
        private readonly ILogger<UpdateSubscriptionCommandHandler> _logger;

        public UpdateSubscriptionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            IStripeService stripeService,
            ILogger<UpdateSubscriptionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _stripeService = stripeService;
            _logger = logger;
        }

        public async Task<SubscriptionDto?> Handle(UpdateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating subscription Id: {SubscriptionId}", request.Id);

            var sub = await _unitOfWork.Subscriptions.GetByIdAsync<long>(request.Id, cancellationToken);
            if (sub == null)
            {
                _logger.LogWarning("Subscription not found. Id: {SubscriptionId}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<SubscriptionDto>(sub);

            // If StripeSubscriptionId exists and the plan changes, sync it to Stripe
            if (!string.IsNullOrEmpty(sub.StripeSubscriptionId) && 
                !string.IsNullOrEmpty(request.Dto.PlanType) && 
                sub.PlanType != request.Dto.PlanType)
            {
                _logger.LogInformation("Updating Stripe subscription {StripeSubscriptionId} to plan {PlanType}", 
                    sub.StripeSubscriptionId, request.Dto.PlanType);
                await _stripeService.UpdateSubscriptionAsync(sub.StripeSubscriptionId, request.Dto.PlanType);
            }

            _mapper.Map(request.Dto, sub);
            _unitOfWork.Subscriptions.Update(sub);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Subscription, object>(
                tableName: "Subscriptions",
                recordId: sub.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Subscription updated. Id: {SubscriptionId}", sub.Id);
            return _mapper.Map<SubscriptionDto>(sub);
        }
    }
}
