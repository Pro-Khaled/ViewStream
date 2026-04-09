using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

[PrimaryKey("ListId", "ShowId")]
public partial class SharedListItem
{
    [Key]
    public long ListId { get; set; }

    [Key]
    public long ShowId { get; set; }

    public DateTime? AddedAt { get; set; }

    public long? AddedByProfileId { get; set; }

    [ForeignKey("AddedByProfileId")]
    [InverseProperty("SharedListItems")]
    public virtual Profile? AddedByProfile { get; set; }

    [ForeignKey("ListId")]
    [InverseProperty("SharedListItems")]
    public virtual SharedList List { get; set; } = null!;

    [ForeignKey("ShowId")]
    [InverseProperty("SharedListItems")]
    public virtual Show Show { get; set; } = null!;
}
