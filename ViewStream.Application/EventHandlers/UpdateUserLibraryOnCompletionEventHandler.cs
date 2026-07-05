using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Domain.Events;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.EventHandlers
{
    /// <summary>
    /// Handles EpisodeCompletedEvent by incrementing the EpisodesWatched counter
    /// in the user's library entry for the show.
    /// </summary>
    public class UpdateUserLibraryOnCompletionEventHandler : INotificationHandler<EpisodeCompletedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserLibraryOnCompletionEventHandler> _logger;

        public UpdateUserLibraryOnCompletionEventHandler(IUnitOfWork unitOfWork, ILogger<UpdateUserLibraryOnCompletionEventHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(EpisodeCompletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Episode completed: Profile {ProfileId}, Episode {EpisodeId}, Show {ShowId}",
                notification.ProfileId, notification.EpisodeId, notification.ShowId);

            // Find or create UserLibrary entry for this show
            var libraryEntries = await _unitOfWork.UserLibraries.FindAsync(
                ul => ul.ProfileId == notification.ProfileId && ul.ShowId == notification.ShowId,
                cancellationToken: cancellationToken);

            var entry = libraryEntries.FirstOrDefault();
            if (entry != null)
            {
                entry.EpisodesWatched = (entry.EpisodesWatched ?? 0) + 1;
                entry.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.UserLibraries.Update(entry);
            }
            else
            {
                // Auto-add to library with "watching" status
                var newEntry = new Domain.Entities.UserLibrary
                {
                    ProfileId = notification.ProfileId,
                    ShowId = notification.ShowId,
                    Status = "watching",
                    EpisodesWatched = 1,
                    StartedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                    AddedAt = DateTime.UtcNow
                };
                await _unitOfWork.UserLibraries.AddAsync(newEntry, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
