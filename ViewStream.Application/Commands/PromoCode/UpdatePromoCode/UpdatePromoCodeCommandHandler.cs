using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PromoCode.UpdatePromoCode
{
    public class UpdatePromoCodeCommandHandler : IRequestHandler<UpdatePromoCodeCommand, PromoCodeDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdatePromoCodeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PromoCodeDto?> Handle(UpdatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var promo = await _unitOfWork.PromoCodes.GetByIdAsync<int>(request.Id, cancellationToken);
            if (promo == null) return null;

            _mapper.Map(request.Dto, promo);
            _unitOfWork.PromoCodes.Update(promo);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PromoCodeDto>(promo);
        }
    }
}
