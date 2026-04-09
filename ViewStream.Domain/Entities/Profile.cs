using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("UserId", "Name", Name = "UQ_Profiles_UserId_Name", IsUnique = true)]
public partial class Profile
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public bool? IsKids { get; set; }

    [StringLength(100)]
    public string? AvatarIcon { get; set; }

    [StringLength(10)]
    public string? LanguagePref { get; set; }

    public short? MaturityLevel { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Profile")]
    public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();

    [InverseProperty("ReportedByProfile")]
    public virtual ICollection<CommentReport> CommentReports { get; set; } = new List<CommentReport>();

    [InverseProperty("Profile")]
    public virtual ICollection<ContentReport> ContentReports { get; set; } = new List<ContentReport>();

    [InverseProperty("Profile")]
    public virtual ICollection<EpisodeComment> EpisodeComments { get; set; } = new List<EpisodeComment>();

    [InverseProperty("Profile")]
    public virtual ICollection<OfflineDownload> OfflineDownloads { get; set; } = new List<OfflineDownload>();

    [InverseProperty("Profile")]
    public virtual ICollection<PersonalizedRow> PersonalizedRows { get; set; } = new List<PersonalizedRow>();

    [InverseProperty("Profile")]
    public virtual ICollection<PlaybackEvent> PlaybackEvents { get; set; } = new List<PlaybackEvent>();

    [InverseProperty("Profile")]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    [InverseProperty("Profile")]
    public virtual ICollection<SearchLog> SearchLogs { get; set; } = new List<SearchLog>();

    [InverseProperty("AddedByProfile")]
    public virtual ICollection<SharedListItem> SharedListItems { get; set; } = new List<SharedListItem>();

    [InverseProperty("OwnerProfile")]
    public virtual ICollection<SharedList> SharedLists { get; set; } = new List<SharedList>();

    [ForeignKey("UserId")]
    [InverseProperty("Profiles")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Profile")]
    public virtual ICollection<UserInteraction> UserInteractions { get; set; } = new List<UserInteraction>();

    [InverseProperty("Profile")]
    public virtual ICollection<UserLibrary> UserLibraries { get; set; } = new List<UserLibrary>();

    [InverseProperty("Profile")]
    public virtual UserVector? UserVector { get; set; }

    [InverseProperty("Profile")]
    public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();

    [InverseProperty("HostProfile")]
    public virtual ICollection<WatchParty> WatchParties { get; set; } = new List<WatchParty>();

    [InverseProperty("Profile")]
    public virtual ICollection<WatchPartyParticipant> WatchPartyParticipants { get; set; } = new List<WatchPartyParticipant>();
}
