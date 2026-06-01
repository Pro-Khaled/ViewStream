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
using ViewStream.Application.Helpers;

namespace ViewStream.Application.Queries.WatchPartyParticipant
{
    public class GetAdminWatchPartyParticipantsPagedQueryHandler : IRequestHandler<GetAdminWatchPartyParticipantsPagedQuery, PagedResult<WatchPartyParticipantDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAdminWatchPartyParticipantsPagedQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<WatchPartyParticipantDto>> Handle(GetAdminWatchPartyParticipantsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.WatchPartyParticipants.GetQueryable()
                .AsNoTracking()
                .Include(p => p.Profile)
                .AsQueryable();

            if (request.WatchPartyId.HasValue)
            {
                query = query.Where(p => p.PartyId == request.WatchPartyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(p => p.Profile.Name.Contains(term));
            }

            if (request.CreatedFrom.HasValue)
                query = query.Where(p => p.JoinedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(p => p.JoinedAt <= request.CreatedTo.Value);

            var projected = query.ProjectTo<WatchPartyParticipantDto>(_mapper.ConfigurationProvider);

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                projected = projected.OrderByPropertyName(request.SortBy, request.SortDescending);
            }
            else
            {
                projected = projected.OrderByDescending(p => p.JoinedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<WatchPartyParticipantDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
