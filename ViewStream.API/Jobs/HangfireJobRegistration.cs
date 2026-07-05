using Hangfire;
using ViewStream.Infrastructure.Jobs;

namespace ViewStream.API.Jobs
{
    /// <summary>
    /// Central registration point for all Hangfire recurring jobs.
    /// Called from Program.cs after app startup.
    /// </summary>
    public static class HangfireJobRegistration
    {
        public static void RegisterAllRecurringJobs()
        {
            // Phase 2: Subscription renewal — daily at 1 AM UTC
            RecurringJob.AddOrUpdate<SubscriptionRenewalJob>(
                "subscription-renewal",
                job => job.Execute(),
                "0 1 * * *"); // 1:00 AM UTC daily

            // Phase 3: Data deletion processing — hourly
            RecurringJob.AddOrUpdate<DataDeletionProcessingJob>(
                "data-deletion-processing",
                job => job.Execute(),
                "0 * * * *"); // Every hour

            // Phase 3: Auto-approve pending deletion requests (30-day GDPR) — daily at 5 AM UTC
            RecurringJob.AddOrUpdate<AutoApproveDeletionRequestsJob>(
                "auto-approve-deletion-requests",
                job => job.Execute(),
                "0 5 * * *"); // 5:00 AM UTC daily

            // Phase 4: Purge old watch history — daily at 3 AM UTC
            RecurringJob.AddOrUpdate<PurgeOldWatchHistoryJob>(
                "purge-old-watch-history",
                job => job.Execute(),
                "0 3 * * *"); // 3:00 AM UTC daily

            // Phase 7: Expire stale watch parties — hourly
            RecurringJob.AddOrUpdate<ExpireStaleWatchPartiesJob>(
                "expire-stale-watch-parties",
                job => job.Execute(),
                "0 * * * *"); // Every hour

            // Phase 9: Purge old audit logs — daily at 2 AM UTC
            RecurringJob.AddOrUpdate<PurgeOldAuditLogsJob>(
                "purge-old-audit-logs",
                job => job.Execute(),
                "0 2 * * *"); // 2:00 AM UTC daily

            // Phase 9: Deactivate expired promo codes — daily at midnight UTC
            RecurringJob.AddOrUpdate<DeactivateExpiredPromoCodesJob>(
                "deactivate-expired-promo-codes",
                job => job.Execute(),
                "0 0 * * *"); // Midnight UTC daily

            // Phase 6: Sync shows to search engine (Meilisearch) — daily at 4 AM UTC
            RecurringJob.AddOrUpdate<SyncShowsToSearchEngineJob>(
                "sync-shows-to-search-engine",
                job => job.Execute(),
                "0 4 * * *"); // 4:00 AM UTC daily

            // Phase 6: Regenerate recommendations (personalized rows) — daily at 3:30 AM UTC
            RecurringJob.AddOrUpdate<RegenerateRecommendationsJob>(
                "regenerate-recommendations",
                job => job.Execute(),
                "30 3 * * *"); // 3:30 AM UTC daily

            // Phase 6: Search results re-ranking — weekly on Sundays at midnight UTC
            RecurringJob.AddOrUpdate<ReRankSearchResultsJob>(
                "re-rank-search-results",
                job => job.Execute(),
                "0 0 * * 0"); // Midnight on Sunday
        }
    }
}
