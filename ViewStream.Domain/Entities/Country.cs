using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class Country
{
    [Key]
    [StringLength(2)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    public string? Continent { get; set; }

    [InverseProperty("CountryCodeNavigation")]
    public virtual ICollection<ShowAvailability> ShowAvailabilities { get; set; } = new List<ShowAvailability>();
}
