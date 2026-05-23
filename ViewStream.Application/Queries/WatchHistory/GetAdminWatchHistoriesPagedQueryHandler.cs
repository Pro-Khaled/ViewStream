using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.WatchHistory
{
    public class GetAdminWatchHistoriesPagedQueryHandler : IRequestHandler<GetAdminWatchHistoriesPagedQuery, PagedResult<AdminWatchHistoryListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminWatchHistoriesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminWatchHistoryListItemDto>> Handle(GetAdminWatchHistoriesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.WatchHistories.GetQueryable()
                .AsNoTracking();

            if (request.ProfileId.HasValue)
                query = query.Where(wh => wh.ProfileId == request.ProfileId.Value);

            if (request.ShowId.HasValue)
                query = query.Where(wh => wh.Episode.Season.ShowId == request.ShowId.Value);

            if (request.Completed.HasValue)
                query = query.Where(wh => wh.Completed == request.Completed.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(wh => wh.Episode.Title.Contains(request.SearchTerm) || wh.Profile.Name.Contains(request.SearchTerm));

            if (request.CreatedFrom.HasValue)
                query = query.Where(wh => wh.WatchedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(wh => wh.WatchedAt <= request.CreatedTo.Value);

            var projected = query.ProjectTo<AdminWatchHistoryListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(wh => wh.WatchedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminWatchHistoryListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
