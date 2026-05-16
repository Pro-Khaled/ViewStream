using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ItemVector
{
    // Minimal admin paged query so ViewStream.API compiles.
    // TODO: Implement real admin item-vectors paging (filters/sorting/db query).
    public record GetAdminItemVectorsPagedQuery(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        bool includeDeleted = false,
        long? showId = null
    ) : AdminPagedQuery(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted),
        IRequest<PagedResult<AdminItemVectorListItemDto>>;
}
