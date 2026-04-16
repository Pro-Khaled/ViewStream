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

namespace ViewStream.Application.Queries.PlaybackEvent
{
    public class GetPlaybackEventsPagedQueryHandler : IRequestHandler<GetPlaybackEventsPagedQuery, PagedResult<PlaybackEventDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPlaybackEventsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<PlaybackEventDto>> Handle(GetPlaybackEventsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.PlaybackEvents.GetQueryable();

            if (request.EpisodeId.HasValue)
                query = query.Where(e => e.EpisodeId == request.EpisodeId.Value);
            if (request.ProfileId.HasValue)
                query = query.Where(e => e.ProfileId == request.ProfileId.Value);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.OrderByDescending(e => e.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
                .AsNoTracking().ToListAsync(cancellationToken);

            return new PagedResult<PlaybackEventDto>
            {
                Items = _mapper.Map<List<PlaybackEventDto>>(items),
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
