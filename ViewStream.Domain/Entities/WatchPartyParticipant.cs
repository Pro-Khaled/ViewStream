using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("PartyId", "ProfileId")]
public partial class WatchPartyParticipant
{
    [Key]
    public long PartyId { get; set; }

    [Key]
    public long ProfileId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    [ForeignKey("PartyId")]
    [InverseProperty("WatchPartyParticipants")]
    public virtual WatchParty Party { get; set; } = null!;

    [ForeignKey("ProfileId")]
    [InverseProperty("WatchPartyParticipants")]
    public virtual Profile Profile { get; set; } = null!;
}
