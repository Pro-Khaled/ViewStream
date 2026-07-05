using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subscription.CreateSubscription
{
    using Subscription = ViewStream.Domain.Entities.Subscription;
    using User = ViewStream.Domain.Entities.User;

    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly UserManager<User> _userManager;
        private readonly IStripeService _stripeService;
        private readonly ILogger<CreateSubscriptionCommandHandler> _logger;

        public CreateSubscriptionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            UserManager<User> userManager,
            IStripeService stripeService,
            ILogger<CreateSubscriptionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _userManager = userManager;
            _stripeService = stripeService;
            _logger = logger;
        }

        public async Task<SubscriptionDto> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating subscription for UserId: {UserId}, PlanType: {PlanType}",
                request.UserId, request.Dto.PlanType);

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} was not found.");
            }

            // Look for existing Stripe Customer ID from user's other subscriptions
            var existingSubs = await _unitOfWork.Subscriptions.FindAsync(
                s => s.UserId == request.UserId,
                cancellationToken: cancellationToken);
            
            var customerId = existingSubs.FirstOrDefault(s => !string.IsNullOrEmpty(s.StripeCustomerId))?.StripeCustomerId;

            if (string.IsNullOrEmpty(customerId))
            {
                _logger.LogInformation("Creating new Stripe Customer for User {UserId} ({Email})", user.Id, user.Email);
                customerId = await _stripeService.CreateCustomerAsync(user.Id, user.Email ?? "", user.FullName);
            }

            _logger.LogInformation("Creating Stripe Subscription for Customer {CustomerId}, PlanType {PlanType}",
                customerId, request.Dto.PlanType);
            
            var stripeSubId = await _stripeService.CreateSubscriptionAsync(customerId, request.Dto.PlanType);

            var sub = _mapper.Map<Subscription>(request.Dto);
            sub.UserId = request.UserId;
            sub.Status = "active";
            sub.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
            sub.CreatedAt = DateTime.UtcNow;
            sub.StripeCustomerId = customerId;
            sub.StripeSubscriptionId = stripeSubId;
            sub.EndsAt = DateTime.UtcNow.AddMonths(1); // Standard 1-month billing period

            await _unitOfWork.Subscriptions.AddAsync(sub, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Subscription, object>(
                tableName: "Subscriptions",
                recordId: sub.Id,
                action: "INSERT",
                oldValues: null,
                newValues: new { sub.PlanType, sub.Status, sub.UserId, sub.StartDate, sub.StripeCustomerId, sub.StripeSubscriptionId },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Subscription created with Id: {SubscriptionId}, StripeSubscriptionId: {StripeSubscriptionId}", sub.Id, sub.StripeSubscriptionId);
            return _mapper.Map<SubscriptionDto>(sub);
        }
    }
}
