using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a friendship connection between two users.
    /// </summary>
    public class FriendshipDto
    {
        /// <summary>Gets or sets the ID of the user initiating the request.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the display name of the initiating user.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Gets or sets the full name of the initiating user.</summary>
        public string? UserFullName { get; set; }

        /// <summary>Gets or sets the avatar URL of the initiating user.</summary>
        public string? UserAvatar { get; set; }

        /// <summary>Gets or sets the ID of the friend.</summary>
        public long FriendId { get; set; }

        /// <summary>Gets or sets the display name of the friend.</summary>
        public string FriendName { get; set; } = string.Empty;

        /// <summary>Gets or sets the full name of the friend.</summary>
        public string? FriendFullName { get; set; }

        /// <summary>Gets or sets the avatar URL of the friend.</summary>
        public string? FriendAvatar { get; set; }

        /// <summary>Gets or sets the current status of the friendship (e.g. "pending", "accepted", "blocked").</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the request was sent.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the friendship status was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a user's friend list.
    /// </summary>
    public class FriendshipListItemDto
    {
        /// <summary>Gets or sets the ID of the friend.</summary>
        public long FriendId { get; set; }

        /// <summary>Gets or sets the display name of the friend.</summary>
        public string FriendName { get; set; } = string.Empty;

        /// <summary>Gets or sets the full name of the friend.</summary>
        public string? FriendFullName { get; set; }

        /// <summary>Gets or sets the avatar URL of the friend.</summary>
        public string? FriendAvatar { get; set; }

        /// <summary>Gets or sets the current status of the friendship.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the request was sent.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether this request was incoming to the current user.</summary>
        public bool IsIncoming { get; set; }
    }

    /// <summary>
    /// Request body for sending a new friend request.
    /// </summary>
    public class FriendRequestDto
    {
        /// <summary>Gets or sets the ID of the user to send the request to.</summary>
        [Required(ErrorMessage = "FriendId is required.")]
        public long FriendId { get; set; }
    }

    /// <summary>
    /// Request body for updating the status of an existing friend request.
    /// </summary>
    public class UpdateFriendshipStatusDto
    {
        /// <summary>Gets or sets the updated status (e.g. "accepted", "blocked"). Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Aggregate summary of a user's friendship statistics.
    /// </summary>
    public class FriendshipSummaryDto
    {
        /// <summary>Gets or sets the ID of the user.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the total number of accepted friends.</summary>
        public int FriendCount { get; set; }

        /// <summary>Gets or sets the total number of pending sent requests.</summary>
        public int PendingSentCount { get; set; }

        /// <summary>Gets or sets the total number of pending received requests.</summary>
        public int PendingReceivedCount { get; set; }

        /// <summary>Gets or sets the total number of blocked users.</summary>
        public int BlockedCount { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for friendships shown in the admin dashboard.
    /// </summary>
    public class AdminFriendshipListItemDto
    {
        /// <summary>Gets or sets the ID of the initiating user.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the ID of the friend.</summary>
        public long FriendId { get; set; }

        /// <summary>Gets or sets the display name of the friend.</summary>
        public string? FriendName { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the request was sent.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets the current status of the friendship.</summary>
        public string? Status { get; set; }
    }
}
