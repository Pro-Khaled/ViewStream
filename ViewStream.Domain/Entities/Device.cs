using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("UserId", "DeviceId", Name = "UQ_Devices_UserId_DeviceId", IsUnique = true)]
public partial class Device
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(255)]
    public string DeviceId { get; set; } = null!;

    [StringLength(100)]
    public string? DeviceName { get; set; }

    [StringLength(20)]
    public string? Platform { get; set; }

    public DateTime? LastActive { get; set; }

    public bool? IsTrusted { get; set; }

    [InverseProperty("Device")]
    public virtual ICollection<LoginSession> LoginSessions { get; set; } = new List<LoginSession>();

    [InverseProperty("Device")]
    public virtual ICollection<OfflineDownload> OfflineDownloads { get; set; } = new List<OfflineDownload>();

    [ForeignKey("UserId")]
    [InverseProperty("Devices")]
    public virtual User User { get; set; } = null!;
}
