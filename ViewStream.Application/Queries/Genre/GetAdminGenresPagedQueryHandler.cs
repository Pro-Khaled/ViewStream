using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Genre
{
    public class GetAdminGenresPagedQueryHandler
        : IRequestHandler<GetAdminGenresPagedQuery, PagedResult<AdminGenreListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminGenresPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminGenreListItemDto>> Handle(
            GetAdminGenresPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Genres.GetQueryable();

            query = query.Include(e => e.Shows);

            // Soft-delete filter
            if (!request.IncludeDeleted)
                query = query.Where(g => g.IsDeleted != true);

            // Date-range filters
            if (request.CreatedFrom.HasValue)
                query = query.Where(g => g.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(g => g.CreatedAt <= request.CreatedTo.Value);

            // Text search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(g => g.Name.Contains(request.SearchTerm));

            var projected = query.Select(g => new AdminGenreListItemDto
            {
                Id = g.Id,
                Name = g.Name,
                ShowCount = g.Shows.Count,
                CreatedAt = g.CreatedAt,
                IsDeleted = g.IsDeleted,
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
                projected = projected.OrderBy(g => g.Name);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminGenreListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
