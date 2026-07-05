using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Episode.GetEpisodeStream
{
    /// <summary>
    /// Handles GetEpisodeStreamQuery by:
    /// 1. Checking geo-availability (country code against ShowAvailability)
    /// 2. Enforcing date-range (AvailableFrom / AvailableUntil)
    /// 3. Filtering stream qualities by subscription tier
    /// </summary>
    public class GetEpisodeStreamQueryHandler : IRequestHandler<GetEpisodeStreamQuery, EpisodeStreamDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAvailabilityCache _availabilityCache;
        private readonly ILogger<GetEpisodeStreamQueryHandler> _logger;

        // Quality definitions by subscription plan
        private static readonly Dictionary<string, string[]> QualityByPlan = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Free"]    = ["360p"],
            ["Basic"]   = ["360p", "480p", "720p"],
            ["Standard"]= ["360p", "480p", "720p", "1080p"],
            ["Premium"] = ["360p", "480p", "720p", "1080p", "4K"]
        };

        public GetEpisodeStreamQueryHandler(
            IUnitOfWork unitOfWork,
            IAvailabilityCache availabilityCache,
            ILogger<GetEpisodeStreamQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _availabilityCache = availabilityCache;
            _logger = logger;
        }

        public async Task<EpisodeStreamDto> Handle(GetEpisodeStreamQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting stream for EpisodeId: {EpisodeId}, UserId: {UserId}, Country: {Country}",
                request.EpisodeId, request.UserId, request.UserCountryCode ?? "unknown");

            // 1. Load the episode with Season → Show navigation
            var episodes = await _unitOfWork.Episodes.FindAsync(
                e => e.Id == request.EpisodeId && e.IsDeleted != true && !e.IsHidden,
                include: q => q.Include(e => e.Season).ThenInclude(s => s.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var episode = episodes.FirstOrDefault();
            if (episode == null)
                throw new KeyNotFoundException($"Episode {request.EpisodeId} not found.");

            var showId = episode.Season.ShowId;

            // 2. Geo-filtering: check ShowAvailability for the user's country
            if (!string.IsNullOrEmpty(request.UserCountryCode))
            {
                var availability = await _availabilityCache.GetAsync(showId, request.UserCountryCode);
                if (availability == null)
                {
                    // Cache miss — fetch from DB
                    var availabilities = await _unitOfWork.ShowAvailabilities.FindAsync(
                        sa => sa.ShowId == showId && sa.CountryCode == request.UserCountryCode,
                        asNoTracking: true,
                        cancellationToken: cancellationToken);
                    availability = availabilities.FirstOrDefault();

                    if (availability != null)
                        await _availabilityCache.SetAsync(showId, request.UserCountryCode, availability);
                }

                if (availability == null)
                {
                    _logger.LogWarning("Show {ShowId} is not available in country {Country}", showId, request.UserCountryCode);
                    throw new UnauthorizedAccessException($"This content is not available in your region ({request.UserCountryCode}).");
                }

                // 3. Date-range enforcement
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                if (availability.AvailableFrom.HasValue && today < availability.AvailableFrom.Value)
                {
                    _logger.LogWarning("Show {ShowId} not yet available (from {From})", showId, availability.AvailableFrom);
                    throw new KeyNotFoundException($"This content is not yet available. Available from {availability.AvailableFrom.Value:yyyy-MM-dd}.");
                }
                if (availability.AvailableUntil.HasValue && today > availability.AvailableUntil.Value)
                {
                    _logger.LogWarning("Show {ShowId} availability expired (until {Until})", showId, availability.AvailableUntil);
                    throw new KeyNotFoundException("This content is no longer available in your region.");
                }
            }

            // 4. Determine subscription tier
            var subscriptions = await _unitOfWork.Subscriptions.FindAsync(
                s => s.UserId == request.UserId && s.Status == "active",
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var currentSub = subscriptions.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
            var planType = currentSub?.PlanType ?? "Free";

            // 5. Build allowed qualities based on subscription plan
            var allowedQualities = QualityByPlan.TryGetValue(planType, out var qualities)
                ? qualities.ToList()
                : QualityByPlan["Free"].ToList();

            // 6. Build stream URLs (example based on HLS master URL pattern)
            var streamUrls = new List<StreamUrlDto>();
            foreach (var quality in allowedQualities)
            {
                streamUrls.Add(new StreamUrlDto
                {
                    Quality = quality,
                    Url = BuildStreamUrl(episode.HlsMasterUrl, episode.VideoUrl, quality),
                    BitrateKbps = GetBitrateForQuality(quality)
                });
            }

            return new EpisodeStreamDto
            {
                EpisodeId = episode.Id,
                Title = episode.Title,
                HlsMasterUrl = episode.HlsMasterUrl,
                DurationSeconds = episode.DurationSeconds,
                AllowedQualities = allowedQualities,
                StreamUrls = streamUrls
            };
        }

        /// <summary>
        /// Builds a quality-specific stream URL from the HLS master URL.
        /// If HLS is not available, falls back to the direct video URL.
        /// </summary>
        private static string BuildStreamUrl(string? hlsMasterUrl, string videoUrl, string quality)
        {
            if (!string.IsNullOrEmpty(hlsMasterUrl))
            {
                // Convention: /hls/{episodeId}/{quality}/playlist.m3u8
                var basePath = hlsMasterUrl.Replace("/master.m3u8", "", StringComparison.OrdinalIgnoreCase);
                return $"{basePath}/{quality}/playlist.m3u8";
            }
            return videoUrl;
        }

        private static int GetBitrateForQuality(string quality) => quality switch
        {
            "360p"  => 800,
            "480p"  => 1400,
            "720p"  => 2800,
            "1080p" => 5000,
            "4K"    => 15000,
            _       => 1000
        };
    }
}
