using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.WatchParty
{
    public record GetAdminWatchPartiesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminWatchPartyListItemDto>>
    {
        public bool? IsActive { get; init; }
        public long? EpisodeId { get; init; }
        public long? HostProfileId { get; init; }

        public GetAdminWatchPartiesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, bool? isActive = null, long? episodeId = null, long? hostProfileId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            IsActive = isActive;
            EpisodeId = episodeId;
            HostProfileId = hostProfileId;
        }
    }
}
