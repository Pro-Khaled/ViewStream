using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PromoCode.ValidatePromoCode
{
    public class ValidatePromoCodeCommandHandler : IRequestHandler<ValidatePromoCodeCommand, PromoCodeValidationResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ValidatePromoCodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PromoCodeValidationResultDto> Handle(ValidatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var promos = await _unitOfWork.PromoCodes.FindAsync(
                p => p.Code == request.Dto.Code,
                include: q => q.Include(p => p.UserPromoUsages),
                cancellationToken: cancellationToken);
            var promo = promos.FirstOrDefault();

            if (promo == null)
                return new PromoCodeValidationResultDto { IsValid = false, Message = "Invalid promo code." };

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (today < promo.ValidFrom)
                return new PromoCodeValidationResultDto { IsValid = false, Message = "Promo code not yet valid." };

            if (promo.ValidUntil.HasValue && today > promo.ValidUntil.Value)
                return new PromoCodeValidationResultDto { IsValid = false, Message = "Promo code expired." };

            if (promo.MaxUses.HasValue && promo.UsedCount >= promo.MaxUses.Value)
                return new PromoCodeValidationResultDto { IsValid = false, Message = "Promo code usage limit reached." };

            if (!string.IsNullOrEmpty(promo.AppliesToPlan) && !string.IsNullOrEmpty(request.Dto.PlanType))
            {
                var allowedPlans = promo.AppliesToPlan.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (!allowedPlans.Contains(request.Dto.PlanType, StringComparer.OrdinalIgnoreCase))
                    return new PromoCodeValidationResultDto { IsValid = false, Message = $"Promo code valid only for plans: {promo.AppliesToPlan}." };
            }

            var dto = _mapper.Map<PromoCodeDto>(promo);
            dto.IsValid = true;
            dto.RemainingUses = promo.MaxUses.HasValue ? promo.MaxUses.Value - promo.UsedCount.GetValueOrDefault() : int.MaxValue;

            decimal discount = 0;
            if (promo.DiscountPercent.HasValue)
                discount = promo.DiscountPercent.Value / 100m;
            else if (promo.DiscountAmount.HasValue)
                discount = promo.DiscountAmount.Value;

            return new PromoCodeValidationResultDto
            {
                IsValid = true,
                PromoCode = dto,
                DiscountAmount = discount
            };
        }
    }
}
