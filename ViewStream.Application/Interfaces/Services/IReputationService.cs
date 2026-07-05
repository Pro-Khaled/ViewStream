namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Manages reputation scores for users based on moderation outcomes.
    /// </summary>
    public interface IReputationService
    {
        /// <summary>
        /// Adjusts the user's reputation score by the given delta.
        /// Valid report: -10. Invalid report (reporter penalized): +5.
        /// Checks thresholds: ≤50 → 7-day suspension, ≤0 → permanent ban.
        /// </summary>
        Task AdjustReputationAsync(long userId, int delta, CancellationToken cancellationToken = default);

        /// <summary>Returns the current reputation score for the user.</summary>
        Task<int> GetReputationAsync(long userId, CancellationToken cancellationToken = default);
    }
}
