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

namespace ViewStream.Application.Queries.UserPromoUsage
{
    public class GetUserPromoUsagesQueryHandler : IRequestHandler<GetUserPromoUsagesQuery, List<UserPromoUsageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserPromoUsagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UserPromoUsageDto>> Handle(GetUserPromoUsagesQuery request, CancellationToken cancellationToken)
        {
            var usages = await _unitOfWork.UserPromoUsages.FindAsync(
                u => u.UserId == request.UserId,
                include: q => q.Include(u => u.User).Include(u => u.PromoCode),
                asNoTracking: true, cancellationToken: cancellationToken);
            return _mapper.Map<List<UserPromoUsageDto>>(usages);
        }
    }
}
