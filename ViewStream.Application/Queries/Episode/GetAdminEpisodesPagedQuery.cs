using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Episode
{
    public record GetAdminEpisodesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminEpisodeListItemDto>>
    {
        public long? ShowId { get; init; }
        public long? SeasonId { get; init; }

        public GetAdminEpisodesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, long? showId = null, long? seasonId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ShowId = showId;
            SeasonId = seasonId;
        }
    }
}
