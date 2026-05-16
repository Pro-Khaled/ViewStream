using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs
{
    /// <summary>
    /// Details of a user's interaction with content, used to feed recommendation algorithms.
    /// </summary>
    public class UserInteractionDto
    {
        /// <summary>Gets or sets the unique identifier of the interaction.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the ID of the profile that performed the interaction.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the ID of the show interacted with.</summary>
        public long ShowId { get; set; }

        /// <summary>Gets or sets the title of the show interacted with.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of interaction (e.g. "view", "like", "share", "add_to_list").</summary>
        public string InteractionType { get; set; } = string.Empty;

        /// <summary>Gets or sets a numerical weight indicating the strength of the interaction.</summary>
        public decimal? Weight { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the interaction occurred.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Slim list-item DTO for a user interaction.
    /// </summary>
    public class UserInteractionListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the interaction.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string ProfileName { get; set; } = string.Empty;

        /// <summary>Gets or sets the title of the show.</summary>
        public string ShowTitle { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of interaction.</summary>
        public string InteractionType { get; set; } = string.Empty;

        /// <summary>Gets or sets the UTC timestamp when the interaction occurred.</summary>
        public DateTime? CreatedAt { get; set; }
    }

    /// <summary>
    /// Request body for logging a new user interaction from a client.
    /// </summary>
    public class CreateUserInteractionDto
    {
        /// <summary>Gets or sets the ID of the show interacted with.</summary>
        [Required(ErrorMessage = "ShowId is required.")]
        public long ShowId { get; set; }

        /// <summary>Gets or sets the type of interaction. Maximum 50 characters.</summary>
        [Required(ErrorMessage = "InteractionType is required.")]
        [MaxLength(50, ErrorMessage = "InteractionType cannot exceed 50 characters.")]
        public string InteractionType { get; set; } = string.Empty;

        /// <summary>Gets or sets an optional numerical weight for the interaction.</summary>
        public decimal? Weight { get; set; } = 1.0m;
    }

    /// <summary>
    /// Aggregated summary of a profile's interactions.
    /// </summary>
    public class UserInteractionSummaryDto
    {
        /// <summary>Gets or sets the ID of the profile.</summary>
        public long ProfileId { get; set; }

        /// <summary>Gets or sets the total number of interactions recorded.</summary>
        public int TotalInteractions { get; set; }

        /// <summary>Gets or sets a dictionary breaking down the total interaction count by type.</summary>
        public Dictionary<string, int> InteractionsByType { get; set; } = new();

        /// <summary>Gets or sets a list of the profile's most frequently interacted-with shows.</summary>
        public List<TopShowDto> TopShows { get; set; } = new();

        /// <summary>
        /// Nested DTO representing a top show for the profile.
        /// </summary>
        public class TopShowDto
        {
            /// <summary>Gets or sets the ID of the show.</summary>
            public long ShowId { get; set; }

            /// <summary>Gets or sets the title of the show.</summary>
            public string ShowTitle { get; set; } = string.Empty;

            /// <summary>Gets or sets the total number of interactions with this show.</summary>
            public int InteractionCount { get; set; }
        }
    }

    /// <summary>
    /// Admin list-item DTO for user interactions shown in the admin dashboard.
    /// </summary>
    public class AdminUserInteractionListItemDto
    {
        /// <summary>Gets or sets the unique identifier of the interaction.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the display name of the profile.</summary>
        public string? ProfileName { get; set; }

        /// <summary>Gets or sets the title of the show.</summary>
        public string? ShowTitle { get; set; }

        /// <summary>Gets or sets the type of interaction.</summary>
        public string? InteractionType { get; set; }

        /// <summary>Gets or sets the UTC timestamp when the interaction occurred.</summary>
        public DateTime? CreatedAt { get; set; }
    }
}
