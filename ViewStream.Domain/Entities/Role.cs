using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("Name", Name = "UQ__Roles__737584F6F8E85D53", IsUnique = true)]
public partial class Role : IdentityRole<long>
{
    [Key]
    public long Id { get; set; }

    [StringLength(256)]
    public string Name { get; set; } = null!;

    [StringLength(256)]
    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    //[InverseProperty("Role")]
    //public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();

    //[InverseProperty("Role")]
    //public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
