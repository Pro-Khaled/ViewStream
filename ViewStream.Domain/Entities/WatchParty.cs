using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("PartyCode", Name = "UQ__WatchPar__39A9713DF754EFCA", IsUnique = true)]
public partial class WatchParty
{
    [Key]
    public long Id { get; set; }

    public long HostProfileId { get; set; }

    public long EpisodeId { get; set; }

    [StringLength(20)]
    public string PartyCode { get; set; } = null!;

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public bool? IsActive { get; set; }

    [ForeignKey("EpisodeId")]
    [InverseProperty("WatchParties")]
    public virtual Episode Episode { get; set; } = null!;

    [ForeignKey("HostProfileId")]
    [InverseProperty("WatchParties")]
    public virtual Profile HostProfile { get; set; } = null!;

    [InverseProperty("Party")]
    public virtual ICollection<WatchPartyParticipant> WatchPartyParticipants { get; set; } = new List<WatchPartyParticipant>();
}
