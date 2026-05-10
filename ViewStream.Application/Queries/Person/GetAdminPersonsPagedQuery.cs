using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Person
{
    public record GetAdminPersonsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminPersonListItemDto>>
    {


        public GetAdminPersonsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {

        }
    }
}
