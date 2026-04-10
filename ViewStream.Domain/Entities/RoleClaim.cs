using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class RoleClaim : IdentityRoleClaim<long>
{
    //[Key]
    //public int Id { get; set; }

    //public long RoleId { get; set; }

    //public string? ClaimType { get; set; }

    //public string? ClaimValue { get; set; }

    //[ForeignKey("RoleId")]
    //[InverseProperty("RoleClaims")]
    //public virtual Role Role { get; set; } = null!;
}
