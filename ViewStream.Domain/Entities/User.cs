using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("NormalizedEmail", Name = "IX_Users_NormalizedEmail")]
public partial class User : IdentityUser<long>
{
    //[Key]
    //public long Id { get; set; }

    //public string? UserName { get; set; }

    //public string? NormalizedUserName { get; set; }

    //[StringLength(256)]
    //public string Email { get; set; } = null!;

    //[StringLength(256)]
    //public string? NormalizedEmail { get; set; }

    //public bool EmailConfirmed { get; set; }

    //public string? PasswordHash { get; set; }

    //public string? SecurityStamp { get; set; }

    //public string? ConcurrencyStamp { get; set; }

    //[StringLength(20)]
    //public string? PhoneNumber { get; set; }

    //public bool PhoneNumberConfirmed { get; set; }

    //public bool TwoFactorEnabled { get; set; }

    //public DateTimeOffset? LockoutEnd { get; set; }

    //public bool LockoutEnabled { get; set; }

    //public int AccessFailedCount { get; set; }

    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(2)]
    [Unicode(false)]
    public string? CountryCode { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public bool IsBlocked { get; set; }

    [StringLength(500)]
    public string? BlockedReason { get; set; }

    public DateTime? BlockedUntil { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("ChangedByUser")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("ReviewedByUser")]
    public virtual ICollection<CommentReport> CommentReports { get; set; } = new List<CommentReport>();

    [InverseProperty("User")]
    public virtual ICollection<DataDeletionRequest> DataDeletionRequests { get; set; } = new List<DataDeletionRequest>();

    [InverseProperty("User")]
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    [InverseProperty("User")]
    public virtual EmailPreference? EmailPreference { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<ErrorLog> ErrorLogs { get; set; } = new List<ErrorLog>();

    [InverseProperty("Friend")]
    public virtual ICollection<Friendship> FriendshipFriends { get; set; } = new List<Friendship>();

    [InverseProperty("User")]
    public virtual ICollection<Friendship> FriendshipUsers { get; set; } = new List<Friendship>();

    [InverseProperty("User")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("User")]
    public virtual ICollection<LoginSession> LoginSessions { get; set; } = new List<LoginSession>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();

    [InverseProperty("User")]
    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();

    [InverseProperty("User")]
    public virtual ICollection<PushToken> PushTokens { get; set; } = new List<PushToken>();

    [InverseProperty("User")]
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    //[InverseProperty("User")]
    //public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    //[InverseProperty("User")]
    //public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    [InverseProperty("User")]
    public virtual ICollection<UserPromoUsage> UserPromoUsages { get; set; } = new List<UserPromoUsage>();

    //[InverseProperty("User")]
    //public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    //[InverseProperty("User")]
    //public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
}
