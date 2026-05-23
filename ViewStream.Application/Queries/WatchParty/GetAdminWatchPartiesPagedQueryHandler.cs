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

namespace ViewStream.Application.Queries.WatchParty
{
    public class GetAdminWatchPartiesPagedQueryHandler : IRequestHandler<GetAdminWatchPartiesPagedQuery, PagedResult<AdminWatchPartyListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminWatchPartiesPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<AdminWatchPartyListItemDto>> Handle(GetAdminWatchPartiesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.WatchParties.GetQueryable()
                .AsNoTracking();

            if (request.IsActive.HasValue)
                query = query.Where(s => s.IsActive == request.IsActive.Value);

            if (request.EpisodeId.HasValue)
                query = query.Where(s => s.EpisodeId == request.EpisodeId.Value);

            if (request.HostProfileId.HasValue)
                query = query.Where(s => s.HostProfileId == request.HostProfileId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(s =>
                    s.PartyCode.Contains(term) ||
                    (s.Episode != null && s.Episode.Title.Contains(term)) ||
                    (s.HostProfile != null && s.HostProfile.Name.Contains(term)));
            }

            if (request.CreatedFrom.HasValue)
                query = query.Where(s => s.StartedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(s => s.StartedAt <= request.CreatedTo.Value);

            var projected = query.ProjectTo<AdminWatchPartyListItemDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(s => s.StartedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminWatchPartyListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
