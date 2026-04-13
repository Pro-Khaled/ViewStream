using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.ContentReport
{
    public record GetContentReportsPagedQuery(
        int Page = 1,
        int PageSize = 20,
        string? Status = null,
        string? TargetType = null
    ) : IRequest<PagedResult<ContentReportListItemDto>>;
}
