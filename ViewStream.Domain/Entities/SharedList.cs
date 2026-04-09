using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[Index("ShareCode", Name = "UQ__SharedLi__20877041F7CA4F8F", IsUnique = true)]
public partial class SharedList
{
    [Key]
    public long Id { get; set; }

    public long OwnerProfileId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsPublic { get; set; }

    [StringLength(20)]
    public string? ShareCode { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("OwnerProfileId")]
    [InverseProperty("SharedLists")]
    public virtual Profile OwnerProfile { get; set; } = null!;

    [InverseProperty("List")]
    public virtual ICollection<SharedListItem> SharedListItems { get; set; } = new List<SharedListItem>();
}
