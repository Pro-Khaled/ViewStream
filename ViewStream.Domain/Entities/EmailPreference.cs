using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class EmailPreference
{
    [Key]
    public long UserId { get; set; }

    public bool? MarketingEmails { get; set; }

    public bool? NewReleaseAlerts { get; set; }

    public bool? RecommendationEmails { get; set; }

    public bool? AccountUpdates { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("EmailPreference")]
    public virtual User User { get; set; } = null!;
}
