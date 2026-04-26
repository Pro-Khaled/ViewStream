using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Commands.PromoCode.ValidatePromoCode;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserPromoUsage.RedeemPromoCode
{
    using PromoCode = ViewStream.Domain.Entities.PromoCode;
    using UserPromoUsage = ViewStream.Domain.Entities.UserPromoUsage;
    public class RedeemPromoCodeCommandHandler : IRequestHandler<RedeemPromoCodeCommand, UserPromoUsageDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RedeemPromoCodeCommandHandler> _logger;

        public RedeemPromoCodeCommandHandler(
            IUnitOfWork unitOfWork,
            IMediator mediator,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<RedeemPromoCodeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<UserPromoUsageDto> Handle(RedeemPromoCodeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Redeeming promo code '{Code}' for UserId: {UserId}", request.Code, request.UserId);

            var validation = await _mediator.Send(new ValidatePromoCodeCommand(new ValidatePromoCodeDto
            {
                Code = request.Code,
                PlanType = request.PlanType
            }), cancellationToken);

            if (!validation.IsValid)
                throw new InvalidOperationException(validation.Message ?? "Invalid promo code.");

            var existing = await _unitOfWork.UserPromoUsages.FindAsync(
                u => u.UserId == request.UserId && u.PromoCodeId == validation.PromoCode!.Id,
                cancellationToken: cancellationToken);

            if (existing.Any())
                throw new InvalidOperationException("You have already used this promo code.");

            var usage = new UserPromoUsage
            {
                UserId = request.UserId,
                PromoCodeId = validation.PromoCode.Id,
                UsedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserPromoUsages.AddAsync(usage, cancellationToken);

            var promo = await _unitOfWork.PromoCodes.GetByIdAsync<int>(validation.PromoCode.Id, cancellationToken);
            promo.UsedCount = (promo.UsedCount ?? 0) + 1;
            _unitOfWork.PromoCodes.Update(promo);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log for the new usage
            _auditContext.SetAudit<UserPromoUsage, object>(
                tableName: "UserPromoUsages",
                recordId: usage.UserId.GetHashCode() ^ usage.PromoCodeId,
                action: "INSERT",
                oldValues: null,
                newValues: new { usage.UserId, usage.PromoCodeId, usage.UsedAt },
                changedByUserId: request.ActorUserId
            );

            // Also audit the updated promo code usage count
            _auditContext.SetAudit<PromoCode, object>(
                tableName: "PromoCodes",
                recordId: promo.Id,
                action: "UPDATE",
                oldValues: new { UsedCount = promo.UsedCount - 1 },
                newValues: new { UsedCount = promo.UsedCount },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Promo code '{Code}' redeemed by User {UserId}", request.Code, request.UserId);

            var result = await _unitOfWork.UserPromoUsages.FindAsync(
                u => u.UserId == usage.UserId && u.PromoCodeId == usage.PromoCodeId,
                include: q => q.Include(u => u.User).Include(u => u.PromoCode),
                cancellationToken: cancellationToken);

            return _mapper.Map<UserPromoUsageDto>(result.First());
        }
    }
}
