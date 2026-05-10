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


            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Name.Contains(request.SearchTerm));

            

            var projected = query.Select(s => new AdminGenreListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                ShowCount = s.Shows.Count,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "showcount" => desc ? projected.OrderByDescending(x => x.ShowCount) : projected.OrderBy(x => x.ShowCount),
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
