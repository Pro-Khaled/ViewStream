using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Genre
{
    public record GetAdminGenresPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminGenreListItemDto>>
    {


        public GetAdminGenresPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {

        }
    }
}
