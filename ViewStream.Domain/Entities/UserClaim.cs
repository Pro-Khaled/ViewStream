using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class UserClaim : IdentityUserClaim<long>
{
    [Key]
    public int Id { get; set; }

    public long UserId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    //[ForeignKey("UserId")]
    //[InverseProperty("UserClaims")]
    //public virtual User User { get; set; } = null!;
}
