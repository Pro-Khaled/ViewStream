using MediatR;

namespace ViewStream.Domain.Events
{
    /// <summary>
    /// Raised when a user completes an episode (reaches ≥ 90% progress).
    /// Handled by UpdateUserLibraryOnCompletionEventHandler to increment EpisodesWatched.
    /// </summary>
    public record EpisodeCompletedEvent(long ProfileId, long EpisodeId, long ShowId) : INotification;
}
