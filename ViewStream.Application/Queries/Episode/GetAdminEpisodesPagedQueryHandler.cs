using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Episode
{
    public class GetAdminEpisodesPagedQueryHandler
        : IRequestHandler<GetAdminEpisodesPagedQuery, PagedResult<AdminEpisodeListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminEpisodesPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminEpisodeListItemDto>> Handle(
            GetAdminEpisodesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Episodes.GetQueryable();

            query = query.Include(e => e.Season);

            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            if (request.CreatedFrom.HasValue)
                query = query.Where(s => s.CreatedAt >= request.CreatedFrom.Value);
            if (request.CreatedTo.HasValue)
                query = query.Where(s => s.CreatedAt <= request.CreatedTo.Value);

            if (request.UpdatedFrom.HasValue)
                query = query.Where(s => s.UpdatedAt >= request.UpdatedFrom.Value);
            if (request.UpdatedTo.HasValue)
                query = query.Where(s => s.UpdatedAt <= request.UpdatedTo.Value);

            if (request.DeletedFrom.HasValue)
                query = query.Where(s => s.DeletedAt >= request.DeletedFrom.Value);
            if (request.DeletedTo.HasValue)
                query = query.Where(s => s.DeletedAt <= request.DeletedTo.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                query = query.Where(s => s.Title.Contains(request.SearchTerm));

            if (request.ShowId.HasValue)
                query = query.Where(s => s.Season.ShowId == request.ShowId.Value);
            if (request.SeasonId.HasValue)
                query = query.Where(s => s.SeasonId == request.SeasonId.Value);

            var projected = query.Select(s => new AdminEpisodeListItemDto
            {
                Id = s.Id,
                SeasonId = s.SeasonId,
                ShowId = s.Season.ShowId,
                EpisodeNumber = s.EpisodeNumber,
                Title = s.Title,
                Description = s.Description,
                RuntimeSeconds = s.RuntimeSeconds,
                VideoUrl = s.VideoUrl,
                ThumbnailUrl = s.ThumbnailUrl,
                ReleaseDate = s.ReleaseDate,
                UpdatedAt = s.UpdatedAt,
                DeletedAt = s.DeletedAt,
                IsDeleted = s.IsDeleted ?? false,
                CreatedAt = s.CreatedAt,
                ShowTitle = s.Season.Show.Title,
                SeasonNumber = s.Season.SeasonNumber,
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

            return new PagedResult<AdminEpisodeListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
