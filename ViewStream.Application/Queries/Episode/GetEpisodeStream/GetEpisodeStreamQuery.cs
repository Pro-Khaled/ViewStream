using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Episode.GetEpisodeStream
{
    /// <summary>
    /// Query to retrieve streaming URLs for an episode, applying geo-filtering,
    /// date-range enforcement, and subscription-tier quality restrictions.
    /// </summary>
    public record GetEpisodeStreamQuery(
        long EpisodeId,
        long UserId,
        string? UserCountryCode
    ) : IRequest<EpisodeStreamDto>;
}
