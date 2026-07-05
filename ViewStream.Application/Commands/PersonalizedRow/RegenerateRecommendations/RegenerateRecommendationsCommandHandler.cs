using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonalizedRow.RegenerateRecommendations
{
    using PersonalizedRow = ViewStream.Domain.Entities.PersonalizedRow;

    /// <summary>
    /// Generates personalized recommendation rows for a profile using a rule-based engine.
    /// 
    /// Strategy:
    /// - Cold-start (less than 3 interactions): globally trending + top-rated shows
    /// - Warm user: genre affinity, "Because you watched X", trending, top-rated by genre
    /// </summary>
    public class RegenerateRecommendationsCommandHandler
        : IRequestHandler<RegenerateRecommendationsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<RegenerateRecommendationsCommandHandler> _logger;

        private const int MinInteractionsForWarm = 3;
        private const int MaxShowsPerRow = 20;

        public RegenerateRecommendationsCommandHandler(
            IUnitOfWork unitOfWork,
            IAuditContext auditContext,
            ILogger<RegenerateRecommendationsCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(RegenerateRecommendationsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Regenerating recommendations for ProfileId: {ProfileId}", request.ProfileId);

            // Remove existing rows for this profile
            var existing = await _unitOfWork.PersonalizedRows.FindAsync(
                r => r.ProfileId == request.ProfileId,
                asNoTracking: false,
                cancellationToken: cancellationToken);

            _unitOfWork.PersonalizedRows.DeleteRange(existing);

            // Load user interactions for this profile
            var interactions = await _unitOfWork.UserInteractions.GetQueryable()
                .Where(ui => ui.ProfileId == request.ProfileId)
                .Include(ui => ui.Show)
                    .ThenInclude(s => s.Genres)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var rows = new List<PersonalizedRow>();

            if (interactions.Count < MinInteractionsForWarm)
            {
                // ═══ COLD-START STRATEGY ═══
                _logger.LogInformation("Cold-start for ProfileId {ProfileId} ({Count} interactions). Using global trends.",
                    request.ProfileId, interactions.Count);

                rows.AddRange(await GenerateColdStartRows(request.ProfileId, cancellationToken));
            }
            else
            {
                // ═══ WARM USER STRATEGY ═══
                _logger.LogInformation("Warm user for ProfileId {ProfileId} ({Count} interactions). Using genre affinity.",
                    request.ProfileId, interactions.Count);

                rows.AddRange(await GenerateWarmUserRows(request.ProfileId, interactions, cancellationToken));
            }

            if (rows.Count > 0)
            {
                await _unitOfWork.PersonalizedRows.AddRangeAsync(rows, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PersonalizedRow, object>(
                tableName: "PersonalizedRows",
                recordId: request.ProfileId,
                action: "REGENERATE",
                oldValues: null,
                newValues: new { request.ProfileId, RowCount = rows.Count },
                changedByUserId: request.ActorUserId);

            _logger.LogInformation("Recommendations regenerated for ProfileId: {ProfileId} ({Count} rows)",
                request.ProfileId, rows.Count);
            return true;
        }

        /// <summary>
        /// Cold-start: generate rows from globally popular and top-rated shows.
        /// </summary>
        private async Task<List<PersonalizedRow>> GenerateColdStartRows(
            long profileId, CancellationToken cancellationToken)
        {
            var rows = new List<PersonalizedRow>();

            // Row 1: Top Rated shows (by IMDB)
            var topRated = await _unitOfWork.Shows.GetQueryable()
                .Where(s => s.IsDeleted == false)
                .OrderByDescending(s => s.ImdbRating)
                .Take(MaxShowsPerRow)
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            if (topRated.Count > 0)
            {
                rows.Add(new PersonalizedRow
                {
                    ProfileId = profileId,
                    RowName = "Top Rated",
                    ShowIdsJson = JsonSerializer.Serialize(topRated),
                    GeneratedAt = DateTime.UtcNow
                });
            }

            // Row 2: Popular Now (by Rotten Tomatoes score)
            var popular = await _unitOfWork.Shows.GetQueryable()
                .Where(s => s.IsDeleted == false)
                .OrderByDescending(s => s.RottenTomatoesScore)
                .Take(MaxShowsPerRow)
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            if (popular.Count > 0)
            {
                rows.Add(new PersonalizedRow
                {
                    ProfileId = profileId,
                    RowName = "Popular Now",
                    ShowIdsJson = JsonSerializer.Serialize(popular),
                    GeneratedAt = DateTime.UtcNow
                });
            }

            // Row 3: Recently Added
            var recent = await _unitOfWork.Shows.GetQueryable()
                .Where(s => s.IsDeleted == false)
                .OrderByDescending(s => s.CreatedAt)
                .Take(MaxShowsPerRow)
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            if (recent.Count > 0)
            {
                rows.Add(new PersonalizedRow
                {
                    ProfileId = profileId,
                    RowName = "Recently Added",
                    ShowIdsJson = JsonSerializer.Serialize(recent),
                    GeneratedAt = DateTime.UtcNow
                });
            }

            return rows;
        }

        /// <summary>
        /// Warm user: generate rows based on genre affinity, watch history, and recency-weighted interactions.
        /// </summary>
        private async Task<List<PersonalizedRow>> GenerateWarmUserRows(
            long profileId,
            List<Domain.Entities.UserInteraction> interactions,
            CancellationToken cancellationToken)
        {
            var rows = new List<PersonalizedRow>();
            var interactedShowIds = interactions.Select(i => i.ShowId).Distinct().ToHashSet();

            // ── 1. Compute genre affinity (weighted by interaction type & recency) ──
            var genreScores = new Dictionary<long, double>();
            var now = DateTime.UtcNow;

            foreach (var interaction in interactions)
            {
                if (interaction.Show?.Genres == null) continue;

                // Weight by interaction type
                double typeWeight = interaction.InteractionType switch
                {
                    "view" => 1.0,
                    "rating" => 1.5,
                    "search_click" => 0.5,
                    "add_to_library" => 1.2,
                    _ => (double)(interaction.Weight ?? 0.5m)
                };

                // Recency decay: weight = 1 / (1 + days_ago * 0.05)
                var daysAgo = (now - (interaction.CreatedAt ?? now)).TotalDays;
                var recencyWeight = 1.0 / (1.0 + daysAgo * 0.05);

                var combinedWeight = typeWeight * recencyWeight;

                foreach (var genre in interaction.Show.Genres)
                {
                    genreScores.TryAdd(genre.Id, 0);
                    genreScores[genre.Id] += combinedWeight;
                }
            }

            // Sort genres by score descending
            var topGenres = genreScores
                .OrderByDescending(g => g.Value)
                .Take(3)
                .ToList();

            // ── 2. "Because You Watched X" — find the most interacted show and suggest similar ──
            var mostInteractedShow = interactions
                .GroupBy(i => i.Show)
                .Where(g => g.Key != null)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key!)
                .FirstOrDefault();

            if (mostInteractedShow?.Genres?.Any() == true)
            {
                var genreIds = mostInteractedShow.Genres.Select(g => g.Id).ToList();
                var similarShows = await _unitOfWork.Shows.GetQueryable()
                    .Where(s => s.IsDeleted == false
                        && !interactedShowIds.Contains(s.Id)
                        && s.Genres.Any(g => genreIds.Contains(g.Id)))
                    .OrderByDescending(s => s.ImdbRating)
                    .Take(MaxShowsPerRow)
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                if (similarShows.Count > 0)
                {
                    var showTitle = mostInteractedShow.Title?.Length > 30
                        ? mostInteractedShow.Title[..27] + "..."
                        : mostInteractedShow.Title ?? "Your Favorites";

                    rows.Add(new PersonalizedRow
                    {
                        ProfileId = profileId,
                        RowName = $"Because You Watched {showTitle}",
                        ShowIdsJson = JsonSerializer.Serialize(similarShows),
                        GeneratedAt = DateTime.UtcNow
                    });
                }
            }

            // ── 3. Genre-specific rows for top genres ──
            foreach (var (genreId, _) in topGenres.Take(2))
            {
                var genre = await _unitOfWork.Genres.GetByIdAsync<long>(genreId, cancellationToken);
                if (genre == null) continue;

                var genreShows = await _unitOfWork.Shows.GetQueryable()
                    .Where(s => s.IsDeleted == false
                        && !interactedShowIds.Contains(s.Id)
                        && s.Genres.Any(g => g.Id == genreId))
                    .OrderByDescending(s => s.ImdbRating)
                    .Take(MaxShowsPerRow)
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                if (genreShows.Count > 0)
                {
                    var rowName = $"Top {genre.Name}";
                    // Ensure row name fits within 50 chars
                    if (rowName.Length > 50) rowName = rowName[..50];

                    rows.Add(new PersonalizedRow
                    {
                        ProfileId = profileId,
                        RowName = rowName,
                        ShowIdsJson = JsonSerializer.Serialize(genreShows),
                        GeneratedAt = DateTime.UtcNow
                    });
                }
            }

            // ── 4. Trending Now (from recent playback events) ──
            var trendingCutoff = DateTime.UtcNow.AddDays(-7);
            var trendingShowIds = await _unitOfWork.PlaybackEvents.GetQueryable()
                .Where(pe => pe.CreatedAt >= trendingCutoff && pe.EventType == "play")
                .GroupBy(pe => pe.Episode.Season.ShowId)
                .OrderByDescending(g => g.Count())
                .Take(MaxShowsPerRow)
                .Select(g => g.Key)
                .ToListAsync(cancellationToken);

            if (trendingShowIds.Count > 0)
            {
                rows.Add(new PersonalizedRow
                {
                    ProfileId = profileId,
                    RowName = "Trending Now",
                    ShowIdsJson = JsonSerializer.Serialize(trendingShowIds),
                    GeneratedAt = DateTime.UtcNow
                });
            }

            // ── 5. Recommended For You — a mix of top genres not yet watched ──
            var recommendedIds = new HashSet<long>();
            foreach (var (genreId, _) in topGenres)
            {
                var genreShows = await _unitOfWork.Shows.GetQueryable()
                    .Where(s => s.IsDeleted == false
                        && !interactedShowIds.Contains(s.Id)
                        && !recommendedIds.Contains(s.Id)
                        && s.Genres.Any(g => g.Id == genreId))
                    .OrderByDescending(s => s.ImdbRating)
                    .Take(MaxShowsPerRow / topGenres.Count)
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                foreach (var id in genreShows) recommendedIds.Add(id);
            }

            if (recommendedIds.Count > 0)
            {
                rows.Add(new PersonalizedRow
                {
                    ProfileId = profileId,
                    RowName = "Recommended For You",
                    ShowIdsJson = JsonSerializer.Serialize(recommendedIds.Take(MaxShowsPerRow).ToList()),
                    GeneratedAt = DateTime.UtcNow
                });
            }

            return rows;
        }
    }
}
