using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.PersonalizedRow
{
    public record GetAdminPersonalizedRowsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminPersonalizedRowListItemDto>>
    {
        public GetAdminPersonalizedRowsPagedQuery(
            int pageNumber = 1,
            int pageSize = 20,
            string? searchTerm = null,
            string? sortBy = null,
            bool sortDescending = false,
            bool includeDeleted = false
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
        }
    }
}
