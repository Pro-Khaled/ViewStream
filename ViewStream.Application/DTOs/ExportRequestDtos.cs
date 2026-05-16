using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Filter parameters for exporting audit log records to CSV.
    /// </summary>
    public class ExportAuditLogsRequest
    {
        /// <summary>Gets or sets an optional full-text search term applied to TableName, Action and user name.</summary>
        public string? SearchTerm { get; set; }

        /// <summary>Gets or sets an optional filter by table name (exact match).</summary>
        [MaxLength(200, ErrorMessage = "TableName cannot exceed 200 characters.")]
        public string? TableName { get; set; }

        /// <summary>Gets or sets an optional filter by the record ID that was changed.</summary>
        public long? RecordId { get; set; }

        /// <summary>Gets or sets an optional filter by action type (e.g. INSERT, UPDATE, DELETE).</summary>
        [MaxLength(50, ErrorMessage = "Action cannot exceed 50 characters.")]
        public string? Action { get; set; }

        /// <summary>Gets or sets an optional filter by the user who performed the change.</summary>
        public long? ChangedByUserId { get; set; }

        /// <summary>Gets or sets the inclusive start date/time filter for ChangedAt.</summary>
        public DateTime? From { get; set; }

        /// <summary>Gets or sets the inclusive end date/time filter for ChangedAt.</summary>
        public DateTime? To { get; set; }
    }

    /// <summary>
    /// Filter parameters for exporting error log records to CSV.
    /// </summary>
    public class ExportErrorLogsRequest
    {
        /// <summary>Gets or sets an optional full-text search term applied to error message and endpoint.</summary>
        public string? SearchTerm { get; set; }

        /// <summary>Gets or sets an optional filter by error code (exact match).</summary>
        [MaxLength(100, ErrorMessage = "ErrorCode cannot exceed 100 characters.")]
        public string? ErrorCode { get; set; }

        /// <summary>Gets or sets an optional filter by endpoint (exact match).</summary>
        [MaxLength(500, ErrorMessage = "Endpoint cannot exceed 500 characters.")]
        public string? Endpoint { get; set; }

        /// <summary>Gets or sets the inclusive start date/time filter for OccurredAt.</summary>
        public DateTime? From { get; set; }

        /// <summary>Gets or sets the inclusive end date/time filter for OccurredAt.</summary>
        public DateTime? To { get; set; }
    }

    /// <summary>
    /// Filter parameters for exporting search log records to CSV.
    /// </summary>
    public class ExportSearchLogsRequest
    {
        /// <summary>Gets or sets an optional full-text search term applied to the query text and profile name.</summary>
        public string? SearchTerm { get; set; }

        /// <summary>Gets or sets an optional filter by profile ID.</summary>
        public long? ProfileId { get; set; }

        /// <summary>Gets or sets an optional filter by the exact search query string.</summary>
        [MaxLength(500, ErrorMessage = "Query cannot exceed 500 characters.")]
        public string? Query { get; set; }

        /// <summary>Gets or sets the inclusive start date/time filter for SearchAt.</summary>
        public DateTime? From { get; set; }

        /// <summary>Gets or sets the inclusive end date/time filter for SearchAt.</summary>
        public DateTime? To { get; set; }
    }

    /// <summary>
    /// Filter parameters for exporting invoice records to CSV.
    /// </summary>
    public class ExportInvoicesRequest
    {
        /// <summary>Gets or sets an optional full-text search term applied to transaction ID and status.</summary>
        public string? SearchTerm { get; set; }

        /// <summary>Gets or sets an optional filter by user ID.</summary>
        public long? UserId { get; set; }

        /// <summary>Gets or sets an optional filter by invoice status (exact match).</summary>
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string? Status { get; set; }

        /// <summary>Gets or sets the inclusive start date filter for InvoiceDate.</summary>
        public DateOnly? From { get; set; }

        /// <summary>Gets or sets the inclusive end date filter for InvoiceDate.</summary>
        public DateOnly? To { get; set; }
    }

    /// <summary>
    /// Filter parameters for exporting user interaction records to CSV.
    /// </summary>
    public class ExportUserInteractionsRequest
    {
        /// <summary>Gets or sets an optional full-text search term applied to interaction type, show title and profile name.</summary>
        public string? SearchTerm { get; set; }

        /// <summary>Gets or sets an optional filter by profile ID.</summary>
        public long? ProfileId { get; set; }

        /// <summary>Gets or sets an optional filter by show ID.</summary>
        public long? ShowId { get; set; }

        /// <summary>Gets or sets an optional filter by interaction type (exact match).</summary>
        [MaxLength(50, ErrorMessage = "InteractionType cannot exceed 50 characters.")]
        public string? InteractionType { get; set; }

        /// <summary>Gets or sets the inclusive start date/time filter for CreatedAt.</summary>
        public DateTime? FromDate { get; set; }

        /// <summary>Gets or sets the inclusive end date/time filter for CreatedAt.</summary>
        public DateTime? ToDate { get; set; }
    }

    /// <summary>
    /// Filter parameters for exporting playback event records to CSV.
    /// </summary>
    public class ExportPlaybackEventsRequest
    {
        /// <summary>Gets or sets an optional full-text search term applied to event type, episode title and profile name.</summary>
        public string? SearchTerm { get; set; }

        /// <summary>Gets or sets an optional filter by episode ID.</summary>
        public long? EpisodeId { get; set; }

        /// <summary>Gets or sets an optional filter by profile ID.</summary>
        public long? ProfileId { get; set; }

        /// <summary>Gets or sets the inclusive start date/time filter for CreatedAt.</summary>
        public DateTime? From { get; set; }

        /// <summary>Gets or sets the inclusive end date/time filter for CreatedAt.</summary>
        public DateTime? To { get; set; }
    }
}
