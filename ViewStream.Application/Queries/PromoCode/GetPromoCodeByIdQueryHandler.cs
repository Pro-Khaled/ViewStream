using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PromoCode
{
    public class GetPromoCodeByIdQueryHandler : IRequestHandler<GetPromoCodeByIdQuery, PromoCodeDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPromoCodeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PromoCodeDto?> Handle(GetPromoCodeByIdQuery request, CancellationToken cancellationToken)
        {
            var promo = await _unitOfWork.PromoCodes.GetByIdAsync<int>(request.Id, cancellationToken);
            return promo == null ? null : _mapper.Map<PromoCodeDto>(promo);
        }
    }
}
