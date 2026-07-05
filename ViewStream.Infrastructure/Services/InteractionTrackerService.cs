using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// Logs user interactions (views, ratings, search clicks) into the UserInteraction table
    /// for the recommendation engine to consume.
    /// </summary>
    public class InteractionTrackerService : IInteractionTracker
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InteractionTrackerService> _logger;

        public InteractionTrackerService(IUnitOfWork unitOfWork, ILogger<InteractionTrackerService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task TrackViewAsync(long profileId, long showId)
        {
            await LogInteractionAsync(profileId, showId, "view", 1.0m);
        }

        public async Task TrackRatingAsync(long profileId, long showId, short rating)
        {
            // Weight: rating normalized to 0-2 scale (1-5 rating → 0.2-1.0 → scaled to weight)
            var weight = rating / 5.0m * 2.0m;
            await LogInteractionAsync(profileId, showId, "rating", weight);
        }

        public async Task TrackSearchClickAsync(long profileId, long showId, string query)
        {
            await LogInteractionAsync(profileId, showId, "search_click", 1.5m);
        }

        private async Task LogInteractionAsync(long profileId, long showId, string type, decimal weight)
        {
            var interaction = new UserInteraction
            {
                ProfileId = profileId,
                ShowId = showId,
                InteractionType = type,
                Weight = weight,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserInteractions.AddAsync(interaction);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogDebug("Interaction logged: {Type} for Profile {ProfileId} on Show {ShowId}", type, profileId, showId);
        }
    }
}
