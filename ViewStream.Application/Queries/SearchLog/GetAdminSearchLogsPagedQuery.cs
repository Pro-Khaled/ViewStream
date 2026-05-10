using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.SearchLog
{
    public record GetAdminSearchLogsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminSearchLogListItemDto>>
    {
        public long? ProfileId { get; init; }
        public string? Query { get; init; }

        public GetAdminSearchLogsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, long? profileId = null, string? query = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            ProfileId = profileId;
            Query = query;
        }
    }
}
