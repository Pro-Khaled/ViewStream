using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.AudioTrack
{
    public record GetAdminAudioTracksPagedQuery : AdminPagedQuery, IRequest<PagedResult<AdminAudioTrackListItemDto>>
    {
        public long? EpisodeId { get; init; }
        public string? LanguageCode { get; init; }
        public bool? IsDefault { get; init; }

        public GetAdminAudioTracksPagedQuery(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? sortBy = null, bool sortDescending = false, bool includeDeleted = false, long? episodeId = null, string? languageCode = null, bool? isDefault = null
        ) : base(pageNumber, pageSize, searchTerm, sortBy, sortDescending, includeDeleted)
        {
            EpisodeId = episodeId;
            LanguageCode = languageCode;
            IsDefault = isDefault;
        }
    }
}
