using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("UserId", "FriendId")]
public partial class Friendship
{
    [Key]
    public long UserId { get; set; }

    [Key]
    public long FriendId { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("FriendId")]
    [InverseProperty("FriendshipFriends")]
    public virtual User Friend { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("FriendshipUsers")]
    public virtual User User { get; set; } = null!;
}
