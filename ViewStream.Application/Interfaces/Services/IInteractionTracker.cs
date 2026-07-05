namespace ViewStream.Application.Interfaces.Services
{
    /// <summary>
    /// Tracks user interactions (views, ratings, search clicks) for the recommendation engine.
    /// </summary>
    public interface IInteractionTracker
    {
        Task TrackViewAsync(long profileId, long showId);
        Task TrackRatingAsync(long profileId, long showId, short rating);
        Task TrackSearchClickAsync(long profileId, long showId, string query);
    }
}
