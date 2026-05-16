using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ShowAward
{
    public record GetAdminShowAwardsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminShowAwardListItemDto>>
    {
        public long? ShowId { get; init; }
        public int? AwardId { get; init; }

        public GetAdminShowAwardsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? showId = null, int? awardId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ShowId = showId;
            AwardId = awardId;
        }
    }
}
