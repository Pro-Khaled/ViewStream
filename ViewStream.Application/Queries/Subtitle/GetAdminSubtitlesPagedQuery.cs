using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Subtitle
{
    public record GetAdminSubtitlesPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminSubtitleListItemDto>>
    {
        public long? EpisodeId { get; init; }
        public string? LanguageCode { get; init; }
        public bool? IsCc { get; init; }

        public GetAdminSubtitlesPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, long? episodeId = null, string? languageCode = null, bool? isCc = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            EpisodeId = episodeId;
            LanguageCode = languageCode;
            IsCc = isCc;
        }
    }
}
