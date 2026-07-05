using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Infrastructure.Jobs
{
    /// <summary>
    /// Hourly Hangfire job that processes approved data deletion requests.
    /// For each request:
    /// 1. Deletes related data (WatchHistory, Ratings, Comments, PlaybackEvents, UserInteractions, SearchLogs)
    /// 2. Anonymizes user record (clears email, hashes personal data, sets IsActive=false)
    /// 3. Deletes or anonymizes profiles
    /// 4. Updates request status to 'completed'
    /// 5. Sends confirmation email
    /// </summary>
    public class DataDeletionProcessingJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DataDeletionProcessingJob> _logger;

        public DataDeletionProcessingJob(IServiceScopeFactory scopeFactory, ILogger<DataDeletionProcessingJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInformation("Data deletion processing job started.");
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var approvedRequests = await unitOfWork.DataDeletionRequests.FindAsync(
                r => r.Status == "approved",
                include: q => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                    .Include(q, r => r.User));

            foreach (var request in approvedRequests)
            {
                try
                {
                    _logger.LogInformation("Processing data deletion for User {UserId}, Request {RequestId}",
                        request.UserId, request.Id);

                    await unitOfWork.BeginTransactionAsync();

                    var userId = request.UserId;
                    var userEmail = request.User?.Email; // Capture before anonymization

                    // 1. Delete watch histories
                    var watchHistories = await unitOfWork.WatchHistories.FindAsync(
                        wh => wh.Profile.UserId == userId);
                    unitOfWork.WatchHistories.DeleteRange(watchHistories);

                    // 2. Delete ratings
                    var ratings = await unitOfWork.Ratings.FindAsync(
                        r => r.Profile.UserId == userId);
                    unitOfWork.Ratings.DeleteRange(ratings);

                    // 3. Delete comments
                    var comments = await unitOfWork.EpisodeComments.FindAsync(
                        c => c.Profile.UserId == userId);
                    unitOfWork.EpisodeComments.DeleteRange(comments);

                    // 4. Delete playback events
                    var playbackEvents = await unitOfWork.PlaybackEvents.FindAsync(
                        pe => pe.Profile.UserId == userId);
                    unitOfWork.PlaybackEvents.DeleteRange(playbackEvents);

                    // 5. Delete user interactions
                    var interactions = await unitOfWork.UserInteractions.FindAsync(
                        ui => ui.Profile.UserId == userId);
                    unitOfWork.UserInteractions.DeleteRange(interactions);

                    // 6. Delete search logs
                    var searchLogs = await unitOfWork.SearchLogs.FindAsync(
                        sl => sl.Profile != null && sl.Profile.UserId == userId);
                    unitOfWork.SearchLogs.DeleteRange(searchLogs);

                    // 7. Anonymize profiles
                    var profiles = await unitOfWork.Profiles.FindAsync(p => p.UserId == userId);
                    foreach (var profile in profiles)
                    {
                        profile.Name = $"Deleted User {profile.Id}";
                        profile.AvatarIcon = null;
                        profile.IsDeleted = true;
                        profile.DeletedAt = DateTime.UtcNow;
                        unitOfWork.Profiles.Update(profile);
                    }

                    // 8. Anonymize user
                    if (request.User != null)
                    {
                        request.User.Email = $"deleted_{userId}@anonymized.viewstream";
                        request.User.NormalizedEmail = request.User.Email.ToUpperInvariant();
                        request.User.FullName = "Deleted User";
                        request.User.PhoneNumber = null;
                        request.User.IsActive = false;
                        request.User.IsDeleted = true;
                        request.User.DeletedAt = DateTime.UtcNow;
                        request.User.UserName = $"deleted_{userId}";
                        request.User.NormalizedUserName = request.User.UserName.ToUpperInvariant();
                    }

                    // 9. Update deletion request status
                    request.Status = "completed";
                    request.CompletedAt = DateTime.UtcNow;
                    unitOfWork.DataDeletionRequests.Update(request);

                    await unitOfWork.SaveChangesAsync();
                    await unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation("Data deletion completed for User {UserId}, Request {RequestId}",
                        userId, request.Id);

                    // 10. Send confirmation email (to original email, captured before anonymization)
                    if (!string.IsNullOrEmpty(userEmail) && !userEmail.Contains("@anonymized.viewstream"))
                    {
                        await emailService.SendNotificationAsync(
                            userEmail,
                            "Data Deletion Complete",
                            "Your data has been permanently deleted from ViewStream as requested. " +
                            "This action is irreversible.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process data deletion for Request {RequestId}", request.Id);
                    await unitOfWork.RollbackTransactionAsync();
                }
            }

            _logger.LogInformation("Data deletion processing job completed.");
        }
    }
}
