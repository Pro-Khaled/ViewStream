using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.CommentReport
{
    public record GetAdminCommentReportsPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminCommentReportListItemDto>>
    {
        public string? Status { get; init; }

        public GetAdminCommentReportsPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, string? status = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            Status = status;
        }
    }
}
