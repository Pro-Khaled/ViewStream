using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.ContentTag
{
    public class GetAdminContentTagsPagedQueryHandler
        : IRequestHandler<GetAdminContentTagsPagedQuery, PagedResult<AdminContentTagListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminContentTagsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminContentTagListItemDto>> Handle(
            GetAdminContentTagsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.ContentTags.GetQueryable();

            query = query.Include(e => e.Shows);

            // Soft-delete filter
            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            // Date-range filters
            if (request.CreatedFrom.HasValue)
                query = query.Where(s => s.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(s => s.CreatedAt <= request.CreatedTo.Value);

            // Text search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Name.Contains(request.SearchTerm));

            // Category filter
            if (!string.IsNullOrWhiteSpace(request.Category))
                query = query.Where(s => s.Category == request.Category);

            var projected = query.Select(s => new AdminContentTagListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                Category = s.Category,
                ShowCount = s.Shows.Count,
                CreatedAt = s.CreatedAt,
                IsDeleted = s.IsDeleted,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "showcount" => desc ? projected.OrderByDescending(x => x.ShowCount) : projected.OrderBy(x => x.ShowCount),
                    "createdat" => desc ? projected.OrderByDescending(x => x.CreatedAt) : projected.OrderBy(x => x.CreatedAt),
                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderBy(s => s.Name);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminContentTagListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
