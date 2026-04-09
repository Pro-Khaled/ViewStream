using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("SessionToken", Name = "UQ__LoginSes__46BDD124D09EBF84", IsUnique = true)]
public partial class LoginSession
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long? DeviceId { get; set; }

    [StringLength(255)]
    public string SessionToken { get; set; } = null!;

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    [ForeignKey("DeviceId")]
    [InverseProperty("LoginSessions")]
    public virtual Device? Device { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("LoginSessions")]
    public virtual User User { get; set; } = null!;
}
