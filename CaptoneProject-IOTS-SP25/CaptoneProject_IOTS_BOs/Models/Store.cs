using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class Store
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int OwnerId { get; set; }

    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }
    
    [MaxLength(500)]
    [Column("image_url")]
    public string? ImageUrl { set; get; }

    public int? IsActive { get; set; }

    [JsonIgnore]
    public virtual User Owner { get; set; }

    public virtual IEnumerable<StoreAttachment>? StoreAttachmentsNavigation { set; get; }
}
