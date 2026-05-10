using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ErrorLog
{
    public record GetAdminErrorLogsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminErrorLogListItemDto>>
    {
        public string? ErrorCode { get; init; }
        public string? Endpoint { get; init; }

        public GetAdminErrorLogsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, string? errorCode = null, string? endpoint = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ErrorCode = errorCode;
            Endpoint = endpoint;
        }
    }
}
