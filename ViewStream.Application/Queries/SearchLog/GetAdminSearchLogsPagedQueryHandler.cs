using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.SearchLog
{
    public class GetAdminSearchLogsPagedQueryHandler
        : IRequestHandler<GetAdminSearchLogsPagedQuery, PagedResult<AdminSearchLogListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminSearchLogsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminSearchLogListItemDto>> Handle(
            GetAdminSearchLogsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.SearchLogs.GetQueryable();

            query = query.Include(e => e.Profile);             query = query.Include(e => e.ClickedShow);


            

            if (request.ProfileId.HasValue)
                query = query.Where(s => s.ProfileId == request.ProfileId.Value);
            if (!string.IsNullOrWhiteSpace(request.SearchTerm) || !string.IsNullOrWhiteSpace(request.Query))
            {
                var term = (request.SearchTerm ?? request.Query ?? "").Trim();
                query = query.Where(s => s.Query.Contains(term) ||
                                         (s.Profile != null && s.Profile.Name.Contains(term)) ||
                                         (s.ClickedShow != null && s.ClickedShow.Title.Contains(term)));
            }

            var projected = query.Select(s => new AdminSearchLogListItemDto
            {
                Id = s.Id,
                ProfileName = s.Profile != null ? s.Profile.Name : null,
                Query = s.Query,
                ResultsCount = s.ResultsCount,
                ClickedShowTitle = s.ClickedShow != null ? s.ClickedShow.Title : null,
                SearchAt = s.SearchAt,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "profilename" => desc ? projected.OrderByDescending(x => x.ProfileName) : projected.OrderBy(x => x.ProfileName),
                    "clickedshowtitle" => desc ? projected.OrderByDescending(x => x.ClickedShowTitle) : projected.OrderBy(x => x.ClickedShowTitle),
                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderByDescending(s => s.SearchAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminSearchLogListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
