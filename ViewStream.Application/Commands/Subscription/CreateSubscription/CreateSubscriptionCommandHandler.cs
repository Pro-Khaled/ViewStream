using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subscription.CreateSubscription
{
    using Subscription = ViewStream.Domain.Entities.Subscription;
    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateSubscriptionCommandHandler> _logger;

        public CreateSubscriptionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateSubscriptionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<SubscriptionDto> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating subscription for UserId: {UserId}, PlanType: {PlanType}",
                request.UserId, request.Dto.PlanType);

            var sub = _mapper.Map<Subscription>(request.Dto);
            sub.UserId = request.UserId;
            sub.Status = "active";
            sub.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
            sub.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Subscriptions.AddAsync(sub, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Subscription, object>(
                tableName: "Subscriptions",
                recordId: sub.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { sub.PlanType, sub.Status, sub.UserId, sub.StartDate },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Subscription created with Id: {SubscriptionId}", sub.Id);
            return _mapper.Map<SubscriptionDto>(sub);
        }
    }
}
