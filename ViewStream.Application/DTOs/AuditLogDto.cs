using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Full details of an audit log record tracking changes to the database.
    /// </summary>
    public class AuditLogDto
    {
        /// <summary>Gets or sets the unique identifier of the audit log.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the name of the database table modified.</summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>Gets or sets the primary key ID of the record modified.</summary>
        public long RecordId { get; set; }

        /// <summary>Gets or sets the action performed (e.g. "INSERT", "UPDATE", "DELETE").</summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>Gets or sets the serialized JSON representation of the old values, if applicable.</summary>
        public string? OldValues { get; set; }

        /// <summary>Gets or sets the serialized JSON representation of the new values, if applicable.</summary>
        public string? NewValues { get; set; }

        /// <summary>Gets or sets the ID of the user who performed the action.</summary>
        public long? ChangedByUserId { get; set; }

        /// <summary>Gets or sets the display name or email of the user who performed the action.</summary>
        public string? ChangedByUserName { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the change occurred.</summary>
        public DateTime? ChangedAt { get; set; }

        /// <summary>Gets or sets the IP address of the client that requested the change.</summary>
        public string? IpAddress { get; set; }

        /// <summary>Gets or sets the user agent string of the client that requested the change.</summary>
        public string? UserAgent { get; set; }

        /// <summary>Gets or sets optional administrative notes regarding the change.</summary>
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for audit logs.
    /// </summary>
    public class AuditLogListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the audit log.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the name of the database table modified.</summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>Gets or sets the primary key ID of the record modified.</summary>
        public long RecordId { get; set; }

        /// <summary>Gets or sets the action performed.</summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name or email of the user who performed the action.</summary>
        public string? ChangedByUserName { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the change occurred.</summary>
        public DateTime? ChangedAt { get; set; }
    }

    /// <summary>
    /// Request body for manually logging an audit event.
    /// </summary>
    public class CreateAuditLogDto
    {
        /// <summary>Gets or sets the name of the database table modified.</summary>
        [Required(ErrorMessage = "TableName is required.")]
        [MaxLength(100, ErrorMessage = "TableName cannot exceed 100 characters.")]
        public string TableName { get; set; } = string.Empty;

        /// <summary>Gets or sets the primary key ID of the record modified.</summary>
        [Required(ErrorMessage = "RecordId is required.")]
        public long RecordId { get; set; }

        /// <summary>Gets or sets the action performed.</summary>
        [Required(ErrorMessage = "Action is required.")]
        [MaxLength(50, ErrorMessage = "Action cannot exceed 50 characters.")]
        public string Action { get; set; } = string.Empty;

        /// <summary>Gets or sets the serialized JSON representation of the old values.</summary>
        public string? OldValues { get; set; }

        /// <summary>Gets or sets the serialized JSON representation of the new values.</summary>
        public string? NewValues { get; set; }

        /// <summary>Gets or sets the ID of the user who performed the action.</summary>
        public long? ChangedByUserId { get; set; }

        /// <summary>Gets or sets the IP address of the client.</summary>
        [MaxLength(50, ErrorMessage = "IpAddress cannot exceed 50 characters.")]
        public string? IpAddress { get; set; }

        /// <summary>Gets or sets the user agent string.</summary>
        [MaxLength(500, ErrorMessage = "UserAgent cannot exceed 500 characters.")]
        public string? UserAgent { get; set; }

        /// <summary>Gets or sets optional notes.</summary>
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Admin list-item DTO for audit logs shown in the admin dashboard.
    /// </summary>
    public class AdminAuditLogListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the audit log.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the name of the database table modified.</summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>Gets or sets the primary key ID of the record modified.</summary>
        public long RecordId { get; set; }

        /// <summary>Gets or sets the action performed.</summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name or email of the user who performed the action.</summary>
        public string? ChangedByUserName { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the change occurred.</summary>
        public DateTime? ChangedAt { get; set; }
    }
}
