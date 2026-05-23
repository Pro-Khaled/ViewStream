using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a system or user notification.
    /// </summary>
    public class NotificationDto
    {
        /// <summary>Gets or sets the unique identifier of the notification.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user receiving the notification.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the title of the notification.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the body text of the notification.</summary>
        public string? Body { get; set; }

        /// <summary>Gets or sets the category or type (e.g. "System", "Promo", "Billing").</summary>
        public string? NotificationType { get; set; }

        /// <summary>Gets or sets additional contextual data serialized as JSON.</summary>
        public string? DataJson { get; set; }

        /// <summary>Gets or sets a value indicating whether the user has read the notification.</summary>
        public bool? IsRead { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the notification was generated.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Request body for creating a new notification (Admin/System operation).
    /// </summary>
    public class CreateNotificationDto
    {
        /// <summary>Gets or sets the ID of the user receiving the notification.</summary>
        [Required(ErrorMessage = "UserId is required.")]
        public long UserId { get; set; }

        /// <summary>Gets or sets the email of the user receiving the notification (alternative to UserId).</summary>
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        /// <summary>Gets or sets the title of the notification. Maximum 200 characters.</summary>
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the body text. Maximum 2000 characters.</summary>
        [MaxLength(2000, ErrorMessage = "Body cannot exceed 2000 characters.")]
        public string? Body { get; set; }

        /// <summary>Gets or sets the category or type. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "NotificationType cannot exceed 50 characters.")]
        public string? NotificationType { get; set; }

        /// <summary>Gets or sets additional contextual data serialized as JSON.</summary>
        public string? DataJson { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for notifications shown in the admin dashboard.
    /// </summary>
    public class AdminNotificationListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the notification.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user receiving the notification.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the title of the notification.</summary>
        public string? Title { get; set; }

        /// <summary>Gets or sets the body text or message.</summary>
        public string? Message { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the notification was generated.</summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>Gets or sets a value indicating whether the notification was read.</summary>
        public bool? IsRead { get; set; }
        public string? NotificationType { get; set; }
        public string? DataJson { get; set; }
    }
}

