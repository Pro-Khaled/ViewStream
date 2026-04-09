using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using ViewStream.Domain.Entities;

namespace ViewStream.Infrastructure.Persistence;

public partial class ViewStreamDbContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public ViewStreamDbContext()
    {
    }

    public ViewStreamDbContext(DbContextOptions<ViewStreamDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AudioTrack> AudioTracks { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Award> Awards { get; set; }

    public virtual DbSet<CommentLike> CommentLikes { get; set; }

    public virtual DbSet<CommentReport> CommentReports { get; set; }

    public virtual DbSet<ContentReport> ContentReports { get; set; }

    public virtual DbSet<ContentTag> ContentTags { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Credit> Credits { get; set; }

    public virtual DbSet<DataDeletionRequest> DataDeletionRequests { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<EmailPreference> EmailPreferences { get; set; }

    public virtual DbSet<Episode> Episodes { get; set; }

    public virtual DbSet<EpisodeComment> EpisodeComments { get; set; }

    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<ItemVector> ItemVectors { get; set; }

    public virtual DbSet<LoginSession> LoginSessions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<OfflineDownload> OfflineDownloads { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<PersonAward> PersonAwards { get; set; }

    public virtual DbSet<PersonalizedRow> PersonalizedRows { get; set; }

    public virtual DbSet<PlaybackEvent> PlaybackEvents { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<PromoCode> PromoCodes { get; set; }

    public virtual DbSet<PushToken> PushTokens { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<SearchLog> SearchLogs { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<SharedList> SharedLists { get; set; }

    public virtual DbSet<SharedListItem> SharedListItems { get; set; }

    public virtual DbSet<Show> Shows { get; set; }

    public virtual DbSet<ShowAvailability> ShowAvailabilities { get; set; }

    public virtual DbSet<ShowAward> ShowAwards { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Subtitle> Subtitles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserInteraction> UserInteractions { get; set; }

    public virtual DbSet<UserLibrary> UserLibraries { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserPromoUsage> UserPromoUsages { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    public virtual DbSet<UserVector> UserVectors { get; set; }

    public virtual DbSet<WatchHistory> WatchHistories { get; set; }

    public virtual DbSet<WatchParty> WatchParties { get; set; }

    public virtual DbSet<WatchPartyParticipant> WatchPartyParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>().ToTable("UserRoles");
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<UserClaim>().ToTable("UserClaims");
        modelBuilder.Entity<UserLogin>().ToTable("UserLogins");
        modelBuilder.Entity<RoleClaim>().ToTable("RoleClaims");
        modelBuilder.Entity<UserToken>().ToTable("UserTokens");


        modelBuilder.Entity<AudioTrack>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AudioTra__3214EC0723BC2D93");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.LanguageCode).IsFixedLength();
            entity.Property(e => e.TrackType).HasDefaultValue("dub");

            entity.HasOne(d => d.Episode).WithMany(p => p.AudioTracks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AudioTrac__Episo__40F9A68C");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AuditLog__3214EC07B9120EC7");

            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.AuditLogs).HasConstraintName("FK__AuditLogs__Chang__53D770D6");
        });

        modelBuilder.Entity<Award>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Awards__3214EC07C9780B76");
        });

        modelBuilder.Entity<CommentLike>(entity =>
        {
            entity.HasKey(e => new { e.CommentId, e.ProfileId }).HasName("PK__CommentL__91241744C80FEE26");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ReactionType).HasDefaultValue("like");

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentLikes).HasConstraintName("FK__CommentLi__Comme__5F492382");

            entity.HasOne(d => d.Profile).WithMany(p => p.CommentLikes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentLi__Profi__603D47BB");
        });

        modelBuilder.Entity<CommentReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CommentR__3214EC07219A44C7");

            entity.HasIndex(e => e.Status, "IX_CommentReports_Status").HasFilter("([Status]='pending')");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Status).HasDefaultValue("pending");

            entity.HasOne(d => d.Comment).WithMany(p => p.CommentReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentRe__Comme__65F62111");

            entity.HasOne(d => d.ReportedByProfile).WithMany(p => p.CommentReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentRe__Repor__66EA454A");

            entity.HasOne(d => d.ReviewedByUser).WithMany(p => p.CommentReports).HasConstraintName("FK__CommentRe__Revie__6ABAD62E");
        });

        modelBuilder.Entity<ContentReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentR__3214EC07C86474D5");

            entity.Property(e => e.ReportedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Status).HasDefaultValue("pending");

            entity.HasOne(d => d.Episode).WithMany(p => p.ContentReports).HasConstraintName("FK__ContentRe__Episo__4589517F");

            entity.HasOne(d => d.Profile).WithMany(p => p.ContentReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContentRe__Profi__43A1090D");

            entity.HasOne(d => d.Show).WithMany(p => p.ContentReports).HasConstraintName("FK__ContentRe__ShowI__44952D46");
        });

        modelBuilder.Entity<ContentTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentT__3214EC074F9C8EA7");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__Countrie__A25C5AA6B7190258");

            entity.Property(e => e.Code).IsFixedLength();
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Credits__3214EC0777517A30");

            entity.HasOne(d => d.Episode).WithMany(p => p.Credits).HasConstraintName("FK__Credits__Episode__1EA48E88");

            entity.HasOne(d => d.Person).WithMany(p => p.Credits).HasConstraintName("FK__Credits__PersonI__1BC821DD");

            entity.HasOne(d => d.Season).WithMany(p => p.Credits).HasConstraintName("FK__Credits__SeasonI__1DB06A4F");

            entity.HasOne(d => d.Show).WithMany(p => p.Credits).HasConstraintName("FK__Credits__ShowId__1CBC4616");
        });

        modelBuilder.Entity<DataDeletionRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DataDele__3214EC07DE48CE49");

            entity.Property(e => e.RequestedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Status).HasDefaultValue("pending");

            entity.HasOne(d => d.User).WithMany(p => p.DataDeletionRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DataDelet__UserI__4D2A7347");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Devices__3214EC0742FF21B1");

            entity.Property(e => e.IsTrusted).HasDefaultValue(false);
            entity.Property(e => e.LastActive).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Devices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Devices__UserId__318258D2");
        });

        modelBuilder.Entity<EmailPreference>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__EmailPre__1788CC4CE42853B1");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.AccountUpdates).HasDefaultValue(true);
            entity.Property(e => e.MarketingEmails).HasDefaultValue(true);
            entity.Property(e => e.NewReleaseAlerts).HasDefaultValue(true);
            entity.Property(e => e.RecommendationEmails).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithOne(p => p.EmailPreference)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmailPref__UserI__69FBBC1F");
        });

        modelBuilder.Entity<Episode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Episodes__3214EC07465BA3F1");

            entity.HasIndex(e => e.IsDeleted, "IX_Episodes_IsDeleted").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Season).WithMany(p => p.Episodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Episodes__Season__151B244E");
        });

        modelBuilder.Entity<EpisodeComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EpisodeC__3214EC0736497BE3");

            entity.HasIndex(e => e.EpisodeId, "IX_EpisodeComments_EpisodeId").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.ParentCommentId, "IX_EpisodeComments_ParentCommentId").HasFilter("([ParentCommentId] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsEdited).HasDefaultValue(false);

            entity.HasOne(d => d.Episode).WithMany(p => p.EpisodeComments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EpisodeCo__Episo__57A801BA");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment).HasConstraintName("FK__EpisodeCo__Paren__59904A2C");

            entity.HasOne(d => d.Profile).WithMany(p => p.EpisodeComments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EpisodeCo__Profi__589C25F3");
        });

        modelBuilder.Entity<ErrorLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ErrorLog__3214EC07B6FEA7DA");

            entity.Property(e => e.OccurredAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithMany(p => p.ErrorLogs).HasConstraintName("FK__ErrorLogs__UserI__19AACF41");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.FriendId }).HasName("PK__Friendsh__3DA43A14362737CC");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Status).HasDefaultValue("pending");

            entity.HasOne(d => d.Friend).WithMany(p => p.FriendshipFriends)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Friendshi__Frien__719CDDE7");

            entity.HasOne(d => d.User).WithMany(p => p.FriendshipUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Friendshi__UserI__70A8B9AE");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genres__3214EC07330D4828");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invoices__3214EC07A0456B7C");

            entity.Property(e => e.Currency)
                .HasDefaultValue("USD")
                .IsFixedLength();
            entity.Property(e => e.Status).HasDefaultValue("pending");

            entity.HasOne(d => d.Subscription).WithMany(p => p.Invoices).HasConstraintName("FK__Invoices__Subscr__74AE54BC");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Invoices__UserId__73BA3083");
        });

        modelBuilder.Entity<ItemVector>(entity =>
        {
            entity.HasKey(e => e.ShowId).HasName("PK__ItemVect__6DE3E0B23338B579");

            entity.Property(e => e.ShowId).ValueGeneratedNever();
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Show).WithOne(p => p.ItemVector)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemVecto__ShowI__51300E55");
        });

        modelBuilder.Entity<LoginSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoginSes__3214EC074B8D7BC6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Device).WithMany(p => p.LoginSessions).HasConstraintName("FK__LoginSess__Devic__39237A9A");

            entity.HasOne(d => d.User).WithMany(p => p.LoginSessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoginSess__UserI__382F5661");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07DB93714A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__5F7E2DAC");
        });

        modelBuilder.Entity<OfflineDownload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OfflineD__3214EC07DD5935AA");

            entity.Property(e => e.DownloadedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Device).WithMany(p => p.OfflineDownloads)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OfflineDo__Devic__3FD07829");

            entity.HasOne(d => d.Episode).WithMany(p => p.OfflineDownloads)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OfflineDo__Episo__3EDC53F0");

            entity.HasOne(d => d.Profile).WithMany(p => p.OfflineDownloads)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OfflineDo__Profi__3DE82FB7");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC07B8E7EE8B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.LastFour).IsFixedLength();

            entity.HasOne(d => d.User).WithMany(p => p.PaymentMethods)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentMe__UserI__6E01572D");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC07E423F9A3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Persons__3214EC07AA1ED38C");
        });

        modelBuilder.Entity<PersonAward>(entity =>
        {
            entity.HasKey(e => new { e.PersonId, e.AwardId }).HasName("PK__PersonAw__512768BA576C8F43");

            entity.Property(e => e.Won).HasDefaultValue(false);

            entity.HasOne(d => d.Award).WithMany(p => p.PersonAwards)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PersonAwa__Award__2CBDA3B5");

            entity.HasOne(d => d.Person).WithMany(p => p.PersonAwards)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PersonAwa__Perso__2BC97F7C");
        });

        modelBuilder.Entity<PersonalizedRow>(entity =>
        {
            entity.HasKey(e => new { e.ProfileId, e.RowName }).HasName("PK__Personal__B84DEF6EE6F7535A");

            entity.Property(e => e.GeneratedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Profile).WithMany(p => p.PersonalizedRows)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Personali__Profi__5BAD9CC8");
        });

        modelBuilder.Entity<PlaybackEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Playback__3214EC0739FBD2F6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Episode).WithMany(p => p.PlaybackEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaybackE__Episo__14E61A24");

            entity.HasOne(d => d.Profile).WithMany(p => p.PlaybackEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PlaybackE__Profi__13F1F5EB");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Profiles__3214EC07DF6515A8");

            entity.HasIndex(e => e.IsDeleted, "IX_Profiles_IsDeleted").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsKids).HasDefaultValue(false);
            entity.Property(e => e.LanguagePref).HasDefaultValue("en");
            entity.Property(e => e.MaturityLevel).HasDefaultValue((short)18);

            entity.HasOne(d => d.User).WithMany(p => p.Profiles).HasConstraintName("FK__Profiles__UserId__4316F928");
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PromoCod__3214EC073F066A55");

            entity.Property(e => e.UsedCount).HasDefaultValue(0);
        });

        modelBuilder.Entity<PushToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PushToke__3214EC076D132F55");

            entity.Property(e => e.LastUsed).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.User).WithMany(p => p.PushTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PushToken__UserI__65370702");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => new { e.ProfileId, e.ShowId }).HasName("PK__Ratings__1FD2B6EFF6B59EB8");

            entity.Property(e => e.RatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Profile).WithMany(p => p.Ratings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ratings__Profile__2A164134");

            entity.HasOne(d => d.Show).WithMany(p => p.Ratings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ratings__ShowId__2B0A656D");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07A79792D4");

            entity.HasIndex(e => e.NormalizedName, "IX_Roles_NormalizedName")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("FK__RolePermi__Permi__6383C8BA"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK__RolePermi__RoleI__628FA481"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__RolePerm__6400A1A86994377D");
                        j.ToTable("RolePermissions");
                    });
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            //entity.HasKey(e => e.Id).HasName("PK__RoleClai__3214EC073AB0459C");

            //entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims).HasConstraintName("FK__RoleClaim__RoleI__5629CD9C");
        });

        modelBuilder.Entity<SearchLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SearchLo__3214EC07D1C0389A");

            entity.Property(e => e.SearchAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.ClickedShow).WithMany(p => p.SearchLogs).HasConstraintName("FK__SearchLog__Click__10216507");

            entity.HasOne(d => d.Profile).WithMany(p => p.SearchLogs).HasConstraintName("FK__SearchLog__Profi__0F2D40CE");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Seasons__3214EC07B3E751F2");

            entity.HasIndex(e => e.IsDeleted, "IX_Seasons_IsDeleted").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Show).WithMany(p => p.Seasons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seasons__ShowId__0F624AF8");
        });

        modelBuilder.Entity<SharedList>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SharedLi__3214EC07E1B7AD48");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.IsPublic).HasDefaultValue(false);

            entity.HasOne(d => d.OwnerProfile).WithMany(p => p.SharedLists)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SharedLis__Owner__03BB8E22");
        });

        modelBuilder.Entity<SharedListItem>(entity =>
        {
            entity.HasKey(e => new { e.ListId, e.ShowId }).HasName("PK__SharedLi__D55D160EBAF96012");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.AddedByProfile).WithMany(p => p.SharedListItems).HasConstraintName("FK__SharedLis__Added__0C50D423");

            entity.HasOne(d => d.List).WithMany(p => p.SharedListItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SharedLis__ListI__09746778");

            entity.HasOne(d => d.Show).WithMany(p => p.SharedListItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SharedLis__ShowI__0A688BB1");
        });

        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shows__3214EC07209A9206");

            entity.HasIndex(e => e.IsDeleted, "IX_Shows_IsDeleted").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasMany(d => d.Genres).WithMany(p => p.Shows)
                .UsingEntity<Dictionary<string, object>>(
                    "ShowGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .HasConstraintName("FK__ShowGenre__Genre__0B91BA14"),
                    l => l.HasOne<Show>().WithMany()
                        .HasForeignKey("ShowId")
                        .HasConstraintName("FK__ShowGenre__ShowI__0A9D95DB"),
                    j =>
                    {
                        j.HasKey("ShowId", "GenreId").HasName("PK__ShowGenr__9DDBB0E544383F30");
                        j.ToTable("ShowGenres");
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.Shows)
                .UsingEntity<Dictionary<string, object>>(
                    "ShowTag",
                    r => r.HasOne<ContentTag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ShowTags__TagId__22401542"),
                    l => l.HasOne<Show>().WithMany()
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ShowTags__ShowId__214BF109"),
                    j =>
                    {
                        j.HasKey("ShowId", "TagId").HasName("PK__ShowTags__BBB42F28EAFA6B28");
                        j.ToTable("ShowTags");
                    });
        });

        modelBuilder.Entity<ShowAvailability>(entity =>
        {
            entity.HasKey(e => new { e.ShowId, e.CountryCode }).HasName("PK__ShowAvai__A83A5060A3C22196");

            entity.Property(e => e.CountryCode).IsFixedLength();

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.ShowAvailabilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShowAvail__Count__3D2915A8");

            entity.HasOne(d => d.Show).WithMany(p => p.ShowAvailabilities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShowAvail__ShowI__3C34F16F");
        });

        modelBuilder.Entity<ShowAward>(entity =>
        {
            entity.HasKey(e => new { e.ShowId, e.AwardId }).HasName("PK__ShowAwar__96EB73EDF758669B");

            entity.Property(e => e.Won).HasDefaultValue(false);

            entity.HasOne(d => d.Award).WithMany(p => p.ShowAwards)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShowAward__Award__27F8EE98");

            entity.HasOne(d => d.Show).WithMany(p => p.ShowAwards)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShowAward__ShowI__2704CA5F");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC070211634A");

            entity.Property(e => e.AutoRenew).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Status).HasDefaultValue("active");

            entity.HasOne(d => d.User).WithMany(p => p.Subscriptions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__UserI__66603565");
        });

        modelBuilder.Entity<Subtitle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subtitle__3214EC07B002A42C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsCc).HasDefaultValue(false);
            entity.Property(e => e.LanguageCode).IsFixedLength();

            entity.HasOne(d => d.Episode).WithMany(p => p.Subtitles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subtitles__Episo__489AC854");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC077A0132A2");

            entity.HasIndex(e => e.IsActive, "IX_Users_IsActive").HasFilter("([IsActive]=(1))");

            entity.HasIndex(e => new { e.IsDeleted, e.Email }, "IX_Users_IsDeleted_Email").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.CountryCode).IsFixedLength();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsBlocked).HasDefaultValue(false);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.LockoutEnabled).HasDefaultValue(true);
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            //entity.HasKey(e => e.Id).HasName("PK__UserClai__3214EC0704FCA4E4");

            //entity.HasOne(d => d.User).WithMany(p => p.UserClaims).HasConstraintName("FK__UserClaim__UserI__534D60F1");
        });

        modelBuilder.Entity<UserInteraction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserInte__3214EC07FAB3383B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Weight).HasDefaultValue(1.0m);

            entity.HasOne(d => d.Profile).WithMany(p => p.UserInteractions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserInter__Profi__55009F39");

            entity.HasOne(d => d.Show).WithMany(p => p.UserInteractions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserInter__ShowI__55F4C372");
        });

        modelBuilder.Entity<UserLibrary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserLibr__3214EC0740FEA656");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.EpisodesWatched).HasDefaultValue(0);

            entity.HasOne(d => d.Profile).WithMany(p => p.UserLibraries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserLibra__Profi__30C33EC3");

            entity.HasOne(d => d.Season).WithMany(p => p.UserLibraries).HasConstraintName("FK__UserLibra__Seaso__32AB8735");

            entity.HasOne(d => d.Show).WithMany(p => p.UserLibraries).HasConstraintName("FK__UserLibra__ShowI__31B762FC");
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            //entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK__UserLogi__2B2C5B5244D7E952");

            //entity.HasOne(d => d.User).WithMany(p => p.UserLogins).HasConstraintName("FK__UserLogin__UserI__59063A47");
        });

        modelBuilder.Entity<UserPromoUsage>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PromoCodeId }).HasName("PK__UserProm__6FEF70141EA1457A");

            entity.Property(e => e.UsedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.PromoCode).WithMany(p => p.UserPromoUsages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPromo__Promo__00200768");

            entity.HasOne(d => d.User).WithMany(p => p.UserPromoUsages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPromo__UserI__7F2BE32F");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            //entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK__UserRole__AF2760AD64A9A449");

            //entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasConstraintName("FK__UserRoles__RoleI__5070F446");

            //entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasConstraintName("FK__UserRoles__UserI__4F7CD00D");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            //entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK__UserToke__8CC4984128BBC30A");

            //entity.HasOne(d => d.User).WithMany(p => p.UserTokens).HasConstraintName("FK__UserToken__UserI__5BE2A6F2");
        });

        modelBuilder.Entity<UserVector>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserVect__290C88E4B9482E78");

            entity.Property(e => e.ProfileId).ValueGeneratedNever();
            entity.Property(e => e.LastUpdated).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Profile).WithOne(p => p.UserVector)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserVecto__Profi__4D5F7D71");
        });

        modelBuilder.Entity<WatchHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WatchHis__3214EC0797BF91EB");

            entity.Property(e => e.Completed).HasDefaultValue(false);
            entity.Property(e => e.ProgressSeconds).HasDefaultValue(0);
            entity.Property(e => e.WatchedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Episode).WithMany(p => p.WatchHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WatchHist__Episo__245D67DE");

            entity.HasOne(d => d.Profile).WithMany(p => p.WatchHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WatchHist__Profi__236943A5");
        });

        modelBuilder.Entity<WatchParty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WatchPar__3214EC07164BE78C");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StartedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Episode).WithMany(p => p.WatchParties)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WatchPart__Episo__793DFFAF");

            entity.HasOne(d => d.HostProfile).WithMany(p => p.WatchParties)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WatchPart__HostP__7849DB76");
        });

        modelBuilder.Entity<WatchPartyParticipant>(entity =>
        {
            entity.HasKey(e => new { e.PartyId, e.ProfileId }).HasName("PK__WatchPar__44D005BD3AF7CC60");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasOne(d => d.Party).WithMany(p => p.WatchPartyParticipants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WatchPart__Party__7E02B4CC");

            entity.HasOne(d => d.Profile).WithMany(p => p.WatchPartyParticipants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WatchPart__Profi__7EF6D905");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
