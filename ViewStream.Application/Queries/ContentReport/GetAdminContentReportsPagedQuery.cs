using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ContentReport
{
    public record GetAdminContentReportsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminContentReportListItemDto>>
    {
        public string? Status { get; init; }
        public string? TargetType { get; init; }

        public GetAdminContentReportsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, string? status = null, string? targetType = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            Status = status;
            TargetType = targetType;
        }
    }
}
