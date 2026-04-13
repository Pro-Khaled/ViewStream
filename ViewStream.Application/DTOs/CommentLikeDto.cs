using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class CommentLikeDto
    {
        public long CommentId { get; set; }
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string? ReactionType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CommentReactionSummaryDto
    {
        public long CommentId { get; set; }
        public int TotalReactions { get; set; }
        public Dictionary<string, int> ReactionCounts { get; set; } = new();
        public string? CurrentUserReaction { get; set; }
    }

    public class CreateUpdateCommentLikeDto
    {
        public long CommentId { get; set; }
        public string? ReactionType { get; set; } = "like";
    }


}
