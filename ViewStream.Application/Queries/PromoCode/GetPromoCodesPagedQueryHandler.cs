using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PromoCode
{
    public class GetPromoCodesPagedQueryHandler : IRequestHandler<GetPromoCodesPagedQuery, PagedResult<PromoCodeListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPromoCodesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<PromoCodeListItemDto>> Handle(GetPromoCodesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.PromoCodes.GetQueryable();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (request.IncludeExpired != true)
                query = query.Where(p => !p.ValidUntil.HasValue || p.ValidUntil.Value >= today);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.OrderByDescending(p => p.ValidFrom)
                .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
                .AsNoTracking().ToListAsync(cancellationToken);

            return new PagedResult<PromoCodeListItemDto>
            {
                Items = _mapper.Map<List<PromoCodeListItemDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
