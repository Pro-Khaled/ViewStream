using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Season
{
    public class GetAdminSeasonsPagedQueryHandler
        : IRequestHandler<GetAdminSeasonsPagedQuery, PagedResult<AdminSeasonListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminSeasonsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminSeasonListItemDto>> Handle(
            GetAdminSeasonsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Seasons.GetQueryable();

            query = query.Include(e => e.Show);

            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Title.Contains(request.SearchTerm));

            if (request.ShowId.HasValue)
                query = query.Where(s => s.ShowId == request.ShowId.Value);

            var projected = query.Select(s => new AdminSeasonListItemDto
            {
                Id = s.Id,
                SeasonNumber = s.SeasonNumber,
                Title = s.Title,
                Description = s.Description,
                ReleaseDate = s.ReleaseDate,
                IsDeleted = s.IsDeleted ?? false,
                CreatedAt = s.CreatedAt,
                ShowTitle = s.Show.Title,
                EpisodeCount = s.Episodes.Count,
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "episodecount" => desc ? projected.OrderByDescending(x => x.EpisodeCount) : projected.OrderBy(x => x.EpisodeCount),
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

            return new PagedResult<AdminSeasonListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
