using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Analytics
{
    /// <summary>Query for trending shows based on recent playback events with time decay.</summary>
    public record GetTrendingShowsQuery(int Days = 7, int Limit = 20, string? CountryCode = null) : IRequest<List<TrendingShowDto>>;
}
