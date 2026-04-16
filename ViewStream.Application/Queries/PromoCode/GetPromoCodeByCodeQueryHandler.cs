using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PromoCode
{
    public class GetPromoCodeByCodeQueryHandler : IRequestHandler<GetPromoCodeByCodeQuery, PromoCodeDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPromoCodeByCodeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PromoCodeDto?> Handle(GetPromoCodeByCodeQuery request, CancellationToken cancellationToken)
        {
            var promos = await _unitOfWork.PromoCodes.FindAsync(p => p.Code == request.Code, cancellationToken: cancellationToken);
            var promo = promos.FirstOrDefault();
            return promo == null ? null : _mapper.Map<PromoCodeDto>(promo);
        }
    }
}
