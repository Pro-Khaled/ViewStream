#nullable enable
using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ItemVector;

public record GetAdminItemVectorsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminItemVectorListItemDto>>
{
    public long? ShowId { get; init; }

    public GetAdminItemVectorsPagedQuery(
        int pageNumber = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        bool includeDeleted = false,
        long? showId = null
    ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
    {
        ShowId = showId;
    }
}
