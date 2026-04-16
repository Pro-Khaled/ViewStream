using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PromoCode.CreatePromoCode
{
    using PromoCode = ViewStream.Domain.Entities.PromoCode;
    public class CreatePromoCodeCommandHandler : IRequestHandler<CreatePromoCodeCommand, PromoCodeDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePromoCodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PromoCodeDto> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.PromoCodes.FindAsync(p => p.Code == request.Dto.Code, cancellationToken: cancellationToken);
            if (existing.Any()) throw new InvalidOperationException("Promo code already exists.");

            var promo = _mapper.Map<PromoCode>(request.Dto);
            promo.UsedCount = 0;
            await _unitOfWork.PromoCodes.AddAsync(promo, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PromoCodeDto>(promo);
        }
    }
}
