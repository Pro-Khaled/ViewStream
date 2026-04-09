using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("UserId", "DeviceId", Name = "UQ_PushTokens_UserId_DeviceId", IsUnique = true)]
public partial class PushToken
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(255)]
    public string DeviceId { get; set; } = null!;

    [StringLength(500)]
    public string Token { get; set; } = null!;

    [StringLength(20)]
    public string? Platform { get; set; }

    public DateTime? LastUsed { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("PushTokens")]
    public virtual User User { get; set; } = null!;
}
