using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class EpisodeCommentDto
    {
        public long Id { get; set; }
        public long EpisodeId { get; set; }
        public string EpisodeTitle { get; set; } = string.Empty;
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string? ProfileAvatar { get; set; }
        public long? ParentCommentId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public bool? IsEdited { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        public List<EpisodeCommentDto> Replies { get; set; } = new();
    }

    public class EpisodeCommentListItemDto
    {
        public long Id { get; set; }
        public long ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public string? ProfileAvatar { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public bool? IsEdited { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
    }

    public class CreateEpisodeCommentDto
    {
        public long EpisodeId { get; set; }
        public long? ParentCommentId { get; set; }
        public string CommentText { get; set; } = string.Empty;
    }

    public class UpdateEpisodeCommentDto
    {
        public string CommentText { get; set; } = string.Empty;
    }


}
