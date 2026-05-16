using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ContentReport
{
    public class GetAdminContentReportsPagedQueryHandler
        : IRequestHandler<GetAdminContentReportsPagedQuery, PagedResult<AdminContentReportListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminContentReportsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminContentReportListItemDto>> Handle(
            GetAdminContentReportsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ContentReports.GetQueryable();

            query = query.Include(e => e.Profile);             query = query.Include(e => e.Show);             query = query.Include(e => e.Episode);


            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x =>
                    x.Reason.Contains(term) ||
                    x.Profile.Name.Contains(term) ||
                    (x.Show != null && x.Show.Title.Contains(term)) ||
                    (x.Episode != null && x.Episode.Title.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(s => s.Status == request.Status);
if (!string.IsNullOrWhiteSpace(request.TargetType))
                query = query.Where(s => request.TargetType == "Show" ? s.ShowId != null : request.TargetType == "Episode" ? s.EpisodeId != null : true);

            var projected = query.Select(s => new AdminContentReportListItemDto
            {
                Id = s.Id,
                ProfileName = s.Profile.Name,
                TargetType = s.ShowId != null ? "Show" : "Episode",
                TargetTitle = s.Show != null ? s.Show.Title : s.Episode != null ? s.Episode.Title : "",
                Reason = s.Reason,
                Status = s.Status,
                ReportedAt = s.ReportedAt,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "targettype" => desc ? projected.OrderByDescending(x => x.TargetType) : projected.OrderBy(x => x.TargetType),
                    "targettitle" => desc ? projected.OrderByDescending(x => x.TargetTitle) : projected.OrderBy(x => x.TargetTitle),
                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderByDescending(s => s.ReportedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminContentReportListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
