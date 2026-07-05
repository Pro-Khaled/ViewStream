using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Analytics
{
    /// <summary>
    /// Returns trending shows based on PlaybackEvent counts in the last N days
    /// with exponential time decay (more recent views weighted higher).
    /// </summary>
    public class GetTrendingShowsQueryHandler : IRequestHandler<GetTrendingShowsQuery, List<TrendingShowDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTrendingShowsQueryHandler> _logger;

        public GetTrendingShowsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetTrendingShowsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<TrendingShowDto>> Handle(GetTrendingShowsQuery request, CancellationToken cancellationToken)
        {
            var cutoff = DateTime.UtcNow.AddDays(-request.Days);

            var events = await _unitOfWork.PlaybackEvents.GetQueryable()
                .Where(pe => pe.CreatedAt >= cutoff && pe.EventType == "play")
                .Include(pe => pe.Episode)
                    .ThenInclude(e => e.Season)
                    .ThenInclude(s => s.Show)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Apply time decay: weight = 1 / (1 + days_ago * 0.1)
            var now = DateTime.UtcNow;
            var scored = events
                .GroupBy(pe => pe.Episode.Season.Show)
                .Select(g =>
                {
                    var show = g.Key;
                    var trendScore = g.Sum(pe =>
                    {
                        var daysAgo = (now - (pe.CreatedAt ?? now)).TotalDays;
                        return 1.0m / (1.0m + (decimal)daysAgo * 0.1m);
                    });

                    return new TrendingShowDto
                    {
                        ShowId = show.Id,
                        Title = show.Title,
                        PosterUrl = show.PosterUrl,
                        ViewCount = g.Count(),
                        TrendScore = Math.Round(trendScore, 2)
                    };
                })
                .OrderByDescending(t => t.TrendScore)
                .Take(request.Limit)
                .ToList();

            return scored;
        }
    }
}
