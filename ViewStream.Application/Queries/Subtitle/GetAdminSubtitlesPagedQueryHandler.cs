using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Subtitle
{
    public class GetAdminSubtitlesPagedQueryHandler
        : IRequestHandler<GetAdminSubtitlesPagedQuery, PagedResult<AdminSubtitleListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAdminSubtitlesPagedQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<PagedResult<AdminSubtitleListItemDto>> Handle(
            GetAdminSubtitlesPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Subtitles.GetQueryable();

            query = query.Include(e => e.Episode);

            if (!request.IncludeDeleted)
                query = query.Where(s => s.IsDeleted != true);

            

            if (request.EpisodeId.HasValue)
                query = query.Where(s => s.EpisodeId == request.EpisodeId.Value);
if (!string.IsNullOrWhiteSpace(request.LanguageCode))
                query = query.Where(s => s.LanguageCode == request.LanguageCode);
if (request.IsCc.HasValue)
                query = query.Where(s => s.IsCc == request.IsCc.Value);

            var projected = query.Select(s => new AdminSubtitleListItemDto
            {
                Id = s.Id,
                LanguageCode = s.LanguageCode,
                SubtitleUrl = s.SubtitleUrl,
                IsCc = s.IsCc,
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

            return new PagedResult<AdminSubtitleListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
