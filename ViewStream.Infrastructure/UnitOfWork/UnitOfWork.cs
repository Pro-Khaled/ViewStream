using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ViewStream.Domain.Interfaces;
using ViewStream.Infrastructure.Persistence;
using ViewStream.Infrastructure.Repositories;

namespace ViewStream.Infrastructure.UnitOfWorks
{
    /// <summary>
    /// Unit of Work implementation
    /// Handles database transactions and provides repository access
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ViewStreamDbContext _context;
        private IDbContextTransaction? _transaction;

        public IAudioTrackRepository AudioTracks { get; private set; }
        public IAuditLogRepository AuditLogs { get; private set; }
        public IAwardRepository Awards { get; private set; }
        public ICommentLikeRepository CommentLikes { get; private set; }
        public ICommentReportRepository CommentReports { get; private set; }
        public IContentReportRepository ContentReports { get; private set; }
        public IContentTagRepository ContentTags { get; private set; }
        public ICountryRepository Countries { get; private set; }
        public ICreditRepository Credits { get; private set; }
        public IDataDeletionRequestRepository DataDeletionRequests { get; private set; }
        public IDeviceRepository Devices { get; private set; }
        public IEmailPreferenceRepository EmailPreferences { get; private set; }
        public IEpisodeRepository Episodes { get; private set; }
        public IEpisodeCommentRepository EpisodeComments { get; private set; }
        public IErrorLogRepository ErrorLogs { get; private set; }
        public IFriendshipRepository Friendships { get; private set; }
        public IGenreRepository Genres { get; private set; }
        public IInvoiceRepository Invoices { get; private set; }
        public IItemVectorRepository ItemVectors { get; private set; }
        public ILoginSessionRepository LoginSessions { get; private set; }
        public INotificationRepository Notifications { get; private set; }
        public IOfflineDownloadRepository OfflineDownloads { get; private set; }
        public IPaymentMethodRepository PaymentMethods { get; private set; }
        public IPermissionRepository Permissions { get; private set; }
        public IPersonRepository Persons { get; private set; }
        public IPersonalizedRowRepository PersonalizedRows { get; private set; }
        public IPersonAwardRepository PersonAwards { get; private set; }
        public IPlaybackEventRepository PlaybackEvents { get; private set; }
        public IProfileRepository Profiles { get; private set; }
        public IPromoCodeRepository PromoCodes { get; private set; }
        public IPushTokenRepository PushTokens { get; private set; }
        public IRatingRepository Ratings { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public IRoleClaimRepository RoleClaims { get; private set; }
        public ISearchLogRepository SearchLogs { get; private set; }
        public ISeasonRepository Seasons { get; private set; }
        public ISharedListRepository SharedLists { get; private set; }
        public ISharedListItemRepository SharedListItems { get; private set; }
        public IShowRepository Shows { get; private set; }
        public IShowAvailabilityRepository ShowAvailabilities { get; private set; }
        public IShowAwardRepository ShowAwards { get; private set; }
        public ISubscriptionRepository Subscriptions { get; private set; }
        public ISubtitleRepository Subtitles { get; private set; }
        public IUserRepository Users { get; private set; }
        public IUserClaimRepository UserClaims { get; private set; }
        public IUserInteractionRepository UserInteractions { get; private set; }
        public IUserLibraryRepository UserLibrarys { get; private set; }
        public IUserLoginRepository UserLogins { get; private set; }
        public IUserPromoUsageRepository UserPromoUsages { get; private set; }
        public IUserRoleRepository UserRoles { get; private set; }
        public IUserTokenRepository UserTokens { get; private set; }
        public IUserVectorRepository UserVectors { get; private set; }
        public IWatchHistoryRepository WatchHistorys { get; private set; }
        public IWatchPartyRepository WatchPartys { get; private set; }
        public IWatchPartyParticipantRepository WatchPartyParticipants { get; private set; }

        public IRefreshTokenRepository RefreshTokens { get; private set; }

        public UnitOfWork(ViewStreamDbContext context)
        {
            _context = context;
            AudioTracks = new AudioTrackRepository(_context);
            AuditLogs = new AuditLogRepository(_context);
            Awards = new AwardRepository(_context);
            CommentLikes = new CommentLikeRepository(_context);
            CommentReports = new CommentReportRepository(_context);
            ContentReports = new ContentReportRepository(_context);
            ContentTags = new ContentTagRepository(_context);
            Countries = new CountryRepository(_context);
            Credits = new CreditRepository(_context);
            DataDeletionRequests = new DataDeletionRequestRepository(_context);
            Devices = new DeviceRepository(_context);
            EmailPreferences = new EmailPreferenceRepository(_context);
            Episodes = new EpisodeRepository(_context);
            EpisodeComments = new EpisodeCommentRepository(_context);
            ErrorLogs = new ErrorLogRepository(_context);
            Friendships = new FriendshipRepository(_context);
            Genres = new GenreRepository(_context);
            Invoices = new InvoiceRepository(_context);
            ItemVectors = new ItemVectorRepository(_context);
            LoginSessions = new LoginSessionRepository(_context);
            Notifications = new NotificationRepository(_context);
            OfflineDownloads = new OfflineDownloadRepository(_context);
            PaymentMethods = new PaymentMethodRepository(_context);
            Permissions = new PermissionRepository(_context);
            Persons = new PersonRepository(_context);
            PersonalizedRows = new PersonalizedRowRepository(_context);
            PersonAwards = new PersonAwardRepository(_context);
            PlaybackEvents = new PlaybackEventRepository(_context);
            Profiles = new ProfileRepository(_context);
            PromoCodes = new PromoCodeRepository(_context);
            PushTokens = new PushTokenRepository(_context);
            Ratings = new RatingRepository(_context);
            Roles = new RoleRepository(_context);
            RoleClaims = new RoleClaimRepository(_context);
            SearchLogs = new SearchLogRepository(_context);
            Seasons = new SeasonRepository(_context);
            SharedLists = new SharedListRepository(_context);
            SharedListItems = new SharedListItemRepository(_context);
            Shows = new ShowRepository(_context);
            ShowAvailabilities = new ShowAvailabilityRepository(_context);
            ShowAwards = new ShowAwardRepository(_context);
            Subscriptions = new SubscriptionRepository(_context);
            Subtitles = new SubtitleRepository(_context);
            Users = new UserRepository(_context);
            UserClaims = new UserClaimRepository(_context);
            UserInteractions = new UserInteractionRepository(_context);
            UserLibrarys = new UserLibraryRepository(_context);
            UserLogins = new UserLoginRepository(_context);
            UserPromoUsages = new UserPromoUsageRepository(_context);
            UserRoles = new UserRoleRepository(_context);
            UserTokens = new UserTokenRepository(_context);
            UserVectors = new UserVectorRepository(_context);
            WatchHistorys = new WatchHistoryRepository(_context);
            WatchPartys = new WatchPartyRepository(_context);
            WatchPartyParticipants = new WatchPartyParticipantRepository(_context);
            RefreshTokens = new RefreshTokenRepository(_context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
