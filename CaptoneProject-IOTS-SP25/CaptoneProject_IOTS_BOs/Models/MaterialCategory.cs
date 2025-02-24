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

    [MaxLength(1000)]
    public string? Description { set; get; }

    [ForeignKey(nameof(User))]
    public int? CreatedBy { set; get; }
    [ForeignKey("CreatedBy")]
    public virtual User? CreatedByNavigation {set; get;}
    public DateTime? CreatedDate { set; get; } = DateTime.Now;
    [JsonIgnore]
    public int? Orders { get; set; }
    [JsonIgnore]
    public int? IsActive { get; set; } = 1;

    [JsonIgnore]
    public virtual ICollection<IotsDevice>? IotDevices { get; set; }
}
