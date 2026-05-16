using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.PersonAward
{
    public record GetAdminPersonAwardsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminPersonAwardListItemDto>>
    {
        public long? PersonId { get; init; }
        public int? AwardId { get; init; }

        public GetAdminPersonAwardsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false,
            long? personId = null, int? awardId = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            PersonId = personId;
            AwardId = awardId;
        }
    }
}
