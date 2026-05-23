using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a user's data deletion request.
    /// </summary>
    public class DataDeletionRequestDto
    {
        /// <summary>Gets or sets the unique identifier of the deletion request.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user requesting deletion.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the email of the user requesting deletion.</summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the UTC timestamp when the request was made.</summary>
        public DateTime? RequestedAt { get; set; }

        /// <summary>Gets or sets the current status of the request (e.g. "Pending", "Processing", "Completed", "Cancelled").</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the deletion was completed, if applicable.</summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>Gets or sets the secure confirmation code associated with the request.</summary>
        public string? ConfirmationCode { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a user viewing their own deletion requests.
    /// </summary>
    public class DataDeletionRequestListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the deletion request.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the email of the user requesting deletion.</summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the current status of the request.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the request was made.</summary>
        public DateTime? RequestedAt { get; set; }
    }

    /// <summary>
    /// Request body for updating the status of a data deletion request (Admin operation).
    /// </summary>
    public class UpdateDataDeletionRequestDto
    {
        /// <summary>Gets or sets the updated status (e.g. "Completed", "Processing"). Maximum 50 characters.</summary>
        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string? Status { get; set; }

        /// <summary>Gets or sets the optional confirmation code if completing the request. Maximum 100 characters.</summary>
        [MaxLength(100, ErrorMessage = "ConfirmationCode cannot exceed 100 characters.")]
        public string? ConfirmationCode { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for data deletion requests shown in the admin dashboard.
    /// </summary>
    public class AdminDataDeletionRequestListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the deletion request.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the user requesting deletion.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets the email of the user requesting deletion.</summary>
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>Gets or sets the current status of the request.</summary>
        public string? Status { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the request was made.</summary>
        public DateTime? RequestedAt { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the deletion was completed, if applicable.</summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>Gets or sets the secure confirmation code associated with the request.</summary>
        public string? ConfirmationCode { get; set; }
    }
}
