using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Commands.PromoCode.ValidatePromoCode;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.UserPromoUsage.RedeemPromoCode
{
    using UserPromoUsage = ViewStream.Domain.Entities.UserPromoUsage;
    public class RedeemPromoCodeCommandHandler : IRequestHandler<RedeemPromoCodeCommand, UserPromoUsageDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RedeemPromoCodeCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<UserPromoUsageDto> Handle(RedeemPromoCodeCommand request, CancellationToken cancellationToken)
        {
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

            var result = await _unitOfWork.UserPromoUsages.FindAsync(
                u => u.UserId == usage.UserId && u.PromoCodeId == usage.PromoCodeId,
                include: q => q.Include(u => u.User).Include(u => u.PromoCode),
                cancellationToken: cancellationToken);
            return _mapper.Map<UserPromoUsageDto>(result.First());
        }
    }
}
