using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.WatchHistory
{
    public record GetAdminWatchHistoriesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminWatchHistoryListItemDto>>
    {
        public long? ProfileId { get; init; }
        public long? ShowId { get; init; }
        public bool? Completed { get; init; }

        public GetAdminWatchHistoriesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? profileId = null, long? showId = null, bool? completed = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ProfileId = profileId;
            ShowId = showId;
            Completed = completed;
        }
    }
}
