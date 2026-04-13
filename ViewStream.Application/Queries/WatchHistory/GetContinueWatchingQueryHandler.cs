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

namespace ViewStream.Application.Queries.WatchHistory
{
    public class GetContinueWatchingQueryHandler : IRequestHandler<GetContinueWatchingQuery, List<WatchHistoryListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetContinueWatchingQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<WatchHistoryListItemDto>> Handle(GetContinueWatchingQuery request, CancellationToken cancellationToken)
        {
            var histories = await _unitOfWork.WatchHistories.GetQueryable()
                .Where(wh => wh.ProfileId == request.ProfileId && wh.Completed != true)
                .OrderByDescending(wh => wh.WatchedAt)
                .Take(request.Limit)
                .Include(wh => wh.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<WatchHistoryListItemDto>>(histories);
        }
    }
}
