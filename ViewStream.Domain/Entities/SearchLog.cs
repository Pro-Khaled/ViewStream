using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ViewStream.Domain.Entities;

public partial class SearchLog
{
    [Key]
    public long Id { get; set; }

    public long? ProfileId { get; set; }

    [StringLength(255)]
    public string Query { get; set; } = null!;

    public int? ResultsCount { get; set; }

    public long? ClickedShowId { get; set; }

    public DateTime? SearchAt { get; set; }

    [ForeignKey("ClickedShowId")]
    [InverseProperty("SearchLogs")]
    public virtual Show? ClickedShow { get; set; }

    [ForeignKey("ProfileId")]
    [InverseProperty("SearchLogs")]
    public virtual Profile? Profile { get; set; }
}
