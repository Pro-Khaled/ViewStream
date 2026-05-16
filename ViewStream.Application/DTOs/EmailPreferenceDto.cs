using System;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a user's email notification preferences.
    /// </summary>
    public class EmailPreferenceDto
    {
        /// <summary>Gets or sets the ID of the user.</summary>
        public long UserId { get; set; }

        /// <summary>Gets or sets a value indicating whether the user wants to receive marketing emails.</summary>
        public bool MarketingEmails { get; set; }

        /// <summary>Gets or sets a value indicating whether the user wants to receive new release alerts.</summary>
        public bool NewReleaseAlerts { get; set; }

        /// <summary>Gets or sets a value indicating whether the user wants to receive recommendation emails.</summary>
        public bool RecommendationEmails { get; set; }

        /// <summary>Gets or sets a value indicating whether the user wants to receive account update emails.</summary>
        public bool AccountUpdates { get; set; }
    }

    /// <summary>
    /// Request body for updating email notification preferences.
    /// </summary>
    public class UpdateEmailPreferenceDto
    {
        /// <summary>Gets or sets the updated preference for marketing emails.</summary>
        public bool? MarketingEmails { get; set; }

        /// <summary>Gets or sets the updated preference for new release alerts.</summary>
        public bool? NewReleaseAlerts { get; set; }

        /// <summary>Gets or sets the updated preference for recommendation emails.</summary>
        public bool? RecommendationEmails { get; set; }

        /// <summary>Gets or sets the updated preference for account updates.</summary>
        public bool? AccountUpdates { get; set; }
    }
}
