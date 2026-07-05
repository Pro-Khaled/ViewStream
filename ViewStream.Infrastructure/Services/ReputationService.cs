using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Entities;

namespace ViewStream.Infrastructure.Services
{
    /// <summary>
    /// Manages user reputation scores with threshold-based actions:
    /// - Score ≤ 50: 7-day suspension (IsBannedUntil)
    /// - Score ≤ 0: permanent ban (IsBlocked = true)
    /// </summary>
    public class ReputationService : IReputationService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ReputationService> _logger;

        public ReputationService(UserManager<User> userManager, ILogger<ReputationService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task AdjustReputationAsync(long userId, int delta, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("Cannot adjust reputation: User {UserId} not found", userId);
                return;
            }

            var oldScore = user.ReputationScore;
            user.ReputationScore = Math.Max(0, user.ReputationScore + delta);
            _logger.LogInformation("Reputation adjusted for User {UserId}: {OldScore} → {NewScore} (delta: {Delta})",
                userId, oldScore, user.ReputationScore, delta);

            // Check thresholds
            if (user.ReputationScore <= 0 && !user.IsBlocked)
            {
                _logger.LogWarning("User {UserId} reputation ≤ 0. Applying permanent ban.", userId);
                user.IsBlocked = true;
                user.BlockedReason = "Permanent ban due to reputation score reaching zero.";
            }
            else if (user.ReputationScore <= 50 && user.IsBannedUntil == null)
            {
                _logger.LogWarning("User {UserId} reputation ≤ 50. Applying 7-day suspension.", userId);
                user.IsBannedUntil = DateTime.UtcNow.AddDays(7);
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        public async Task<int> GetReputationAsync(long userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.ReputationScore ?? 100;
        }
    }
}
