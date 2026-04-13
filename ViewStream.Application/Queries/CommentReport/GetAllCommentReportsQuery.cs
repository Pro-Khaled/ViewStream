using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.CommentReport
{
    public record GetReportsPagedQuery(
        int Page = 1,
        int PageSize = 20,
        string? Status = null
    ) : IRequest<PagedResult<CommentReportListItemDto>>;
}
