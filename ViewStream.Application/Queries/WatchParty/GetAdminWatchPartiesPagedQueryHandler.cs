using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.WatchParty
{
    public class GetAdminWatchPartiesPagedQueryHandler
        : IRequestHandler<GetAdminWatchPartiesPagedQuery, PagedResult<AdminWatchPartyListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminWatchPartiesPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminWatchPartyListItemDto>> Handle(
            GetAdminWatchPartiesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.WatchParties.GetQueryable();

            query = query.Include(e => e.HostProfile);             query = query.Include(e => e.Episode.Season.Show);


            

            if (request.IsActive.HasValue)
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            if (request.EpisodeId.HasValue)
                query = query.Where(s => s.EpisodeId == request.EpisodeId.Value);
            if (request.HostProfileId.HasValue)
                query = query.Where(s => s.HostProfileId == request.HostProfileId.Value);

            var projected = query.Select(s => new AdminWatchPartyListItemDto
            {
                Id = s.Id,
                HostProfileName = s.HostProfile.Name,
                EpisodeTitle = s.Episode.Title,
                ShowTitle = s.Episode.Season.Show.Title,
                PartyCode = s.PartyCode,
                IsActive = s.IsActive,
                ParticipantCount = s.WatchPartyParticipants.Count,
                StartedAt = s.StartedAt,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "participantcount" => desc ? projected.OrderByDescending(x => x.ParticipantCount) : projected.OrderBy(x => x.ParticipantCount),
                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
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
