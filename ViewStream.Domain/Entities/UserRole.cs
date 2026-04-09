using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("UserId", "RoleId")]
public partial class UserRole : IdentityUserRole<long>
{
    public long Id { get; set; }

    [Key]
    public long UserId { get; set; }

    [Key]
    public long RoleId { get; set; }

    //[ForeignKey("RoleId")]
    //[InverseProperty("UserRoles")]
    //public virtual Role Role { get; set; } = null!;

    //[ForeignKey("UserId")]
    //[InverseProperty("UserRoles")]
    //public virtual User User { get; set; } = null!;
}
