using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of a system error log.
    /// </summary>
    public class ErrorLogDto
    {
        /// <summary>Gets or sets the unique identifier of the error log.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the optional ID of the user who triggered the error.</summary>
        public long? UserId { get; set; }

        /// <summary>Gets or sets the optional email of the user who triggered the error.</summary>
        public string? UserEmail { get; set; }

        /// <summary>Gets or sets the internal error code or HTTP status code.</summary>
        public string? ErrorCode { get; set; }

        /// <summary>Gets or sets the description or message of the error.</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Gets or sets the full stack trace for debugging.</summary>
        public string? StackTrace { get; set; }

        /// <summary>Gets or sets the API endpoint or application path where the error occurred.</summary>
        public string? Endpoint { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the error occurred.</summary>
        public DateTime? OccurredAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a system error log.
    /// </summary>
    public class ErrorLogListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the error log.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the error code.</summary>
        public string? ErrorCode { get; set; }

        /// <summary>Gets or sets the description of the error.</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Gets or sets the endpoint where the error occurred.</summary>
        public string? Endpoint { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the error occurred.</summary>
        public DateTime? OccurredAt { get; set; }
    }

    /// <summary>
    /// Request body for manually logging a system or client-side error.
    /// </summary>
    public class CreateErrorLogDto
    {
        /// <summary>Gets or sets the optional ID of the user who triggered the error.</summary>
        public long? UserId { get; set; }

        /// <summary>Gets or sets the error code. Maximum 50 characters.</summary>
        [MaxLength(50, ErrorMessage = "ErrorCode cannot exceed 50 characters.")]
        public string? ErrorCode { get; set; }

        /// <summary>Gets or sets the description of the error. Maximum 1000 characters.</summary>
        [MaxLength(1000, ErrorMessage = "ErrorMessage cannot exceed 1000 characters.")]
        public string? ErrorMessage { get; set; }

        /// <summary>Gets or sets the stack trace.</summary>
        public string? StackTrace { get; set; }

        /// <summary>Gets or sets the endpoint. Maximum 500 characters.</summary>
        [MaxLength(500, ErrorMessage = "Endpoint cannot exceed 500 characters.")]
        public string? Endpoint { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for error logs shown in the admin dashboard.
    /// </summary>
    public class AdminErrorLogListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the error log.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the error code.</summary>
        public string? ErrorCode { get; set; }

        /// <summary>Gets or sets the description of the error.</summary>
        public string? ErrorMessage { get; set; }

        /// <summary>Gets or sets the endpoint where the error occurred.</summary>
        public string? Endpoint { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the error occurred.</summary>
        public DateTime? OccurredAt { get; set; }

        public long? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? StackTrace { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
