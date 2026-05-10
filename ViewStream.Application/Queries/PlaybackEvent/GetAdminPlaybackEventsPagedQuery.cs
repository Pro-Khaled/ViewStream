using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.PlaybackEvent
{
    public record GetAdminPlaybackEventsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminPlaybackEventListItemDto>>
    {
        public long? EpisodeId { get; init; }
        public long? ProfileId { get; init; }

        public GetAdminPlaybackEventsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, long? episodeId = null, long? profileId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            EpisodeId = episodeId;
            ProfileId = profileId;
        }
    }
}
