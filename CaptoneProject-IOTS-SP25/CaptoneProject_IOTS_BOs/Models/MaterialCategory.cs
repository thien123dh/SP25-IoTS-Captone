using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class MaterialCategory
{
    public int Id { get; set; }

    public string Label { get; set; }

    [MaxLength(500)]
    [Column("image_url")]
    public string? ImageUrl { set; get; }
    public int? Orders { get; set; }
    public int? IsActive { get; set; } = 1;

    [JsonIgnore]
    public virtual ICollection<Material>? Materials { get; set; } = new List<Material>();
}
