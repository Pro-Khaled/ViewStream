using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Award
{
    public record GetAdminAwardsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminAwardListItemDto>>
    {
        public short? Year { get; init; }

        public GetAdminAwardsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, short? year = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            Year = year;
        }
    }
}
