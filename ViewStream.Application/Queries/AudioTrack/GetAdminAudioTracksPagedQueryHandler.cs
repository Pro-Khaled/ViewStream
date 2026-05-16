using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.AudioTrack
{
    public class GetAdminAudioTracksPagedQueryHandler
        : IRequestHandler<GetAdminAudioTracksPagedQuery, PagedResult<AdminAudioTrackListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminAudioTracksPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminAudioTrackListItemDto>> Handle(
            GetAdminAudioTracksPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.AudioTracks.GetQueryable();

            query = query.Include(e => e.Episode);

            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x =>
                    x.LanguageCode.Contains(term) ||
                    x.TrackType.Contains(term) ||
                    x.Episode.Title.Contains(term));
            }

            if (request.EpisodeId.HasValue)
                query = query.Where(s => s.EpisodeId == request.EpisodeId.Value);
            if (!string.IsNullOrWhiteSpace(request.LanguageCode))
                query = query.Where(s => s.LanguageCode == request.LanguageCode);
            if (request.IsDefault.HasValue)
                query = query.Where(s => s.IsDefault == request.IsDefault.Value);

            var projected = query.Select(s => new AdminAudioTrackListItemDto
            {
                Id = s.Id,
                LanguageCode = s.LanguageCode,
                TrackType = s.TrackType,
                IsDefault = s.IsDefault,
                IsDeleted = s.IsDeleted ?? false,
                CreatedAt = s.CreatedAt,
                EpisodeTitle = s.Episode.Title,
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

            return new PagedResult<AdminAudioTrackListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
