using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Show
{
    public class GetAdminShowsPagedQueryHandler
        : IRequestHandler<GetAdminShowsPagedQuery, PagedResult<AdminShowListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminShowsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminShowListItemDto>> Handle(
            GetAdminShowsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Shows.GetQueryable();

            query = query.Include(e => e.Genres);

            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Title.Contains(request.SearchTerm));

            if (request.GenreId.HasValue)
                query = query.Where(s => s.Genres.Any(g => g.Id == request.GenreId.Value));
            if (request.ReleaseYear.HasValue)
                query = query.Where(s => s.ReleaseYear == request.ReleaseYear.Value);

            var projected = query.Select(s => new AdminShowListItemDto
            {
                Id = s.Id,
                Title = s.Title,
                ReleaseYear = s.ReleaseYear,
                MaturityRating = s.MaturityRating,
                PosterUrl = s.PosterUrl,
                ImdbRating = s.ImdbRating,
                Genres = s.Genres.Select(g => g.Name).ToList(),
                IsDeleted = s.IsDeleted ?? false,
                AddedAt = s.AddedAt,
                UpdatedAt = s.UpdatedAt,
                RottenTomatoesScore = s.RottenTomatoesScore,
                SeasonCount = s.Seasons.Count,
                EpisodeCount = s.Seasons.Sum(se => se.Episodes.Count),
            });

            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                bool desc = request.SortDescending;
                projected = request.SortBy.ToLower() switch
                {
                    "seasoncount" => desc ? projected.OrderByDescending(x => x.SeasonCount) : projected.OrderBy(x => x.SeasonCount),
                    "episodecount" => desc ? projected.OrderByDescending(x => x.EpisodeCount) : projected.OrderBy(x => x.EpisodeCount),
                    _ => projected.OrderByPropertyName(request.SortBy, desc)
                };
            }
            else
            {
                projected = projected.OrderByDescending(s => s.AddedAt);
            }

            var totalCount = await projected.CountAsync(cancellationToken);
            var items = await projected
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminShowListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
