using System;
using System.Threading.Tasks;

namespace ViewStream.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern for managing transactions and providing repository access
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>Save all changes in a single transaction</summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>Begin a new transaction</summary>
        Task BeginTransactionAsync();
        
        /// <summary>Commit current transaction</summary>
        Task CommitTransactionAsync();
        
        /// <summary>Rollback current transaction</summary>
        Task RollbackTransactionAsync();

        IAudioTrackRepository AudioTracks { get; }
        IAuditLogRepository AuditLogs { get; }
        IAwardRepository Awards { get; }
        ICommentLikeRepository CommentLikes { get; }
        ICommentReportRepository CommentReports { get; }
        IContentReportRepository ContentReports { get; }
        IContentTagRepository ContentTags { get; }
        ICountryRepository Countrys { get; }
        ICreditRepository Credits { get; }
        IDataDeletionRequestRepository DataDeletionRequests { get; }
        IDeviceRepository Devices { get; }
        IEmailPreferenceRepository EmailPreferences { get; }
        IEpisodeRepository Episodes { get; }
        IEpisodeCommentRepository EpisodeComments { get; }
        IErrorLogRepository ErrorLogs { get; }
        IFriendshipRepository Friendships { get; }
        IGenreRepository Genres { get; }
        IInvoiceRepository Invoices { get; }
        IItemVectorRepository ItemVectors { get; }
        ILoginSessionRepository LoginSessions { get; }
        INotificationRepository Notifications { get; }
        IOfflineDownloadRepository OfflineDownloads { get; }
        IPaymentMethodRepository PaymentMethods { get; }
        IPermissionRepository Permissions { get; }
        IPersonRepository Persons { get; }
        IPersonalizedRowRepository PersonalizedRows { get; }
        IPersonAwardRepository PersonAwards { get; }
        IPlaybackEventRepository PlaybackEvents { get; }
        IProfileRepository Profiles { get; }
        IPromoCodeRepository PromoCodes { get; }
        IPushTokenRepository PushTokens { get; }
        IRatingRepository Ratings { get; }
        IRoleRepository Roles { get; }
        IRoleClaimRepository RoleClaims { get; }
        ISearchLogRepository SearchLogs { get; }
        ISeasonRepository Seasons { get; }
        ISharedListRepository SharedLists { get; }
        ISharedListItemRepository SharedListItems { get; }
        IShowRepository Shows { get; }
        IShowAvailabilityRepository ShowAvailabilitys { get; }
        IShowAwardRepository ShowAwards { get; }
        ISubscriptionRepository Subscriptions { get; }
        ISubtitleRepository Subtitles { get; }
        IUserRepository Users { get; }
        IUserClaimRepository UserClaims { get; }
        IUserInteractionRepository UserInteractions { get; }
        IUserLibraryRepository UserLibrarys { get; }
        IUserLoginRepository UserLogins { get; }
        IUserPromoUsageRepository UserPromoUsages { get; }
        IUserRoleRepository UserRoles { get; }
        IUserTokenRepository UserTokens { get; }
        IUserVectorRepository UserVectors { get; }
        IWatchHistoryRepository WatchHistorys { get; }
        IWatchPartyRepository WatchPartys { get; }
        IWatchPartyParticipantRepository WatchPartyParticipants { get; }

        IRefreshTokenRepository RefreshTokens { get; }
    }
}
