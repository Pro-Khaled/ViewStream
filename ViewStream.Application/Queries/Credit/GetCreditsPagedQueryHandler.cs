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

namespace ViewStream.Application.Queries.Credit
{
    public class GetCreditsPagedQueryHandler : IRequestHandler<GetCreditsPagedQuery, PagedResult<CreditListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetCreditsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PagedResult<CreditListItemDto>> Handle(GetCreditsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Credits.GetQueryable();
            if (!string.IsNullOrWhiteSpace(request.Role))
                query = query.Where(c => c.Role == request.Role);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.OrderBy(c => c.Role).ThenBy(c => c.Person.Name)
                .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
                .Include(c => c.Person)
                .Include(c => c.Show)
                .Include(c => c.Season).ThenInclude(s => s.Show)
                .Include(c => c.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show)
                .AsNoTracking().ToListAsync(cancellationToken);
            return new PagedResult<CreditListItemDto>
            {
                Items = _mapper.Map<List<CreditListItemDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
