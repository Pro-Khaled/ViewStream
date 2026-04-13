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

namespace ViewStream.Application.Queries.WatchHistory
{
    public class GetWatchHistoryPagedQueryHandler : IRequestHandler<GetWatchHistoryPagedQuery, PagedResult<WatchHistoryListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetWatchHistoryPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<WatchHistoryListItemDto>> Handle(GetWatchHistoryPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.WatchHistories.GetQueryable()
                .Where(wh => wh.ProfileId == request.ProfileId);

            var totalCount = await query.CountAsync(cancellationToken);

            var histories = await query
                .OrderByDescending(wh => wh.WatchedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(wh => wh.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<WatchHistoryListItemDto>
            {
                Items = _mapper.Map<List<WatchHistoryListItemDto>>(histories),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
