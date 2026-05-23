using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.CommentReport
{
    public class GetAdminCommentReportsPagedQueryHandler
        : IRequestHandler<GetAdminCommentReportsPagedQuery, PagedResult<AdminCommentReportListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminCommentReportsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminCommentReportListItemDto>> Handle(
            GetAdminCommentReportsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.CommentReports.GetQueryable();

            query = query.Include(e => e.Comment);             query = query.Include(e => e.ReportedByProfile);


            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x =>
                    x.Comment.CommentText.Contains(term) ||
                    x.Reason.Contains(term) ||
                    x.ReportedByProfile.Name.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(s => s.Status == request.Status);

            if (request.CreatedFrom.HasValue)
                query = query.Where(s => s.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(s => s.CreatedAt <= request.CreatedTo.Value);

            var projected = query.Select(s => new AdminCommentReportListItemDto
            {
                Id = s.Id,
                CommentId = s.CommentId,
                CommentText = s.Comment.CommentText,
                Details = s.Details,
                ReportedByProfileName = s.ReportedByProfile.Name,
                Reason = s.Reason,
                Status = s.Status,
                ReviewedByUserId = s.ReviewedByUserId,
                ReviewedByUserName = s.ReviewedByUser != null ? s.ReviewedByUser.UserName : null,
                ReviewedAt = s.ReviewedAt,
                CreatedAt = s.CreatedAt,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {

                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderByDescending(s => s.CreatedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminCommentReportListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
