using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.DTOs
{
    public class FriendshipDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserFullName { get; set; }
        public string? UserAvatar { get; set; }
        public long FriendId { get; set; }
        public string FriendName { get; set; } = string.Empty;
        public string? FriendFullName { get; set; }
        public string? FriendAvatar { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class FriendshipListItemDto
    {
        public long FriendId { get; set; }
        public string FriendName { get; set; } = string.Empty;
        public string? FriendFullName { get; set; }
        public string? FriendAvatar { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsIncoming { get; set; }
    }

    public class FriendRequestDto
    {
        public long FriendId { get; set; }
    }

    public class UpdateFriendshipStatusDto
    {
        public string Status { get; set; } = string.Empty; // "accepted", "blocked"
    }

    public class FriendshipSummaryDto
    {
        public long UserId { get; set; }
        public int FriendCount { get; set; }
        public int PendingSentCount { get; set; }
        public int PendingReceivedCount { get; set; }
        public int BlockedCount { get; set; }
    }


}
