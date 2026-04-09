using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class Notification
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    [StringLength(30)]
    public string? NotificationType { get; set; }

    public string? DataJson { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User User { get; set; } = null!;
}
