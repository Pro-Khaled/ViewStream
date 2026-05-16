using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PlaybackEvent
{
    public class GetAdminPlaybackEventsPagedQueryHandler
        : IRequestHandler<GetAdminPlaybackEventsPagedQuery, PagedResult<AdminPlaybackEventListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminPlaybackEventsPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminPlaybackEventListItemDto>> Handle(
            GetAdminPlaybackEventsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.PlaybackEvents.GetQueryable();

            query = query.Include(e => e.Profile);             query = query.Include(e => e.Episode);


            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x =>
                    x.EventType.Contains(term) ||
                    x.Episode.Title.Contains(term) ||
                    x.Profile.Name.Contains(term));
            }

            if (request.EpisodeId.HasValue)
                query = query.Where(s => s.EpisodeId == request.EpisodeId.Value);
if (request.ProfileId.HasValue)
                query = query.Where(s => s.ProfileId == request.ProfileId.Value);

            var projected = query.Select(s => new AdminPlaybackEventListItemDto
            {
                Id = s.Id,
                ProfileId = s.ProfileId,
                EpisodeId = s.EpisodeId,
                EventType = s.EventType,
                PositionSeconds = s.PositionSeconds,
                Quality = s.Quality,
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

            return new PagedResult<AdminPlaybackEventListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
