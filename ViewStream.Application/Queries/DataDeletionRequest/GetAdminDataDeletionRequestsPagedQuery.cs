using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.DataDeletionRequest
{
    public record GetAdminDataDeletionRequestsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminDataDeletionRequestListItemDto>>
    {
        public string? Status { get; init; }

        public GetAdminDataDeletionRequestsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, string? status = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            Status = status;
        }
    }
}
