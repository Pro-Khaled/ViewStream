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

namespace ViewStream.Application.Queries.WatchParty
{
    public class GetActiveWatchPartiesPagedQueryHandler : IRequestHandler<GetActiveWatchPartiesPagedQuery, PagedResult<WatchPartyListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetActiveWatchPartiesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<WatchPartyListItemDto>> Handle(GetActiveWatchPartiesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.WatchParties.GetQueryable()
                .Where(p => p.IsActive == true);

            var totalCount = await query.CountAsync(cancellationToken);

            var parties = await query
                .OrderByDescending(p => p.StartedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(p => p.HostProfile)
                .Include(p => p.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show)
                .Include(p => p.WatchPartyParticipants)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<WatchPartyListItemDto>
            {
                Items = _mapper.Map<List<WatchPartyListItemDto>>(parties),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
