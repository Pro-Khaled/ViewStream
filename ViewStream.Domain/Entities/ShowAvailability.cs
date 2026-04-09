using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("ShowId", "CountryCode")]
[Table("ShowAvailability")]
public partial class ShowAvailability
{
    [Key]
    public long ShowId { get; set; }

    [Key]
    [StringLength(2)]
    [Unicode(false)]
    public string CountryCode { get; set; } = null!;

    public DateOnly? AvailableFrom { get; set; }

    public DateOnly? AvailableUntil { get; set; }

    public string? LicensingNotes { get; set; }

    [ForeignKey("CountryCode")]
    [InverseProperty("ShowAvailabilities")]
    public virtual Country CountryCodeNavigation { get; set; } = null!;

    [ForeignKey("ShowId")]
    [InverseProperty("ShowAvailabilities")]
    public virtual Show Show { get; set; } = null!;
}
