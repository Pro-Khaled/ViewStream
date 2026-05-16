using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Rating
{
    public record GetAdminRatingsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminRatingListItemDto>>
    {
        public long? ShowId { get; init; }
        public long? ProfileId { get; init; }

        public GetAdminRatingsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? showId = null, long? profileId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ShowId = showId;
            ProfileId = profileId;
        }
    }
}
