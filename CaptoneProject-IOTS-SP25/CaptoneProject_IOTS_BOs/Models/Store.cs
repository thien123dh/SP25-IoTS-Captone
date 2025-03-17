using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class Store
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
    [Column("contact_number")]
    [MaxLength(50)]
    public string? ContactNumber { set; get; }
    [MaxLength(500)]
    public string? Address { set; get; }
    public int ProvinceId { set; get; }
    public int DistrictId { set; get; }
    public int WardId { set; get; }
    public int AddressId { set; get; }
    [MaxLength(500)]
    public string? Summary { set; get; }
    [JsonIgnore]
    public string Description { get; set; } = "";

    public int OwnerId { get; set; }

    [JsonIgnore]
    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    [JsonIgnore]
    public int? CreatedBy { get; set; }

    [JsonIgnore]
    public DateTime? UpdatedDate { get; set; } = DateTime.Now;

    [JsonIgnore]
    public int? UpdatedBy { get; set; }
    
    [Column("image_url")]
    public string? ImageUrl { set; get; }

    [JsonIgnore]
    public int? IsActive { get; set; } = 1;

    [JsonIgnore]
    public virtual User Owner { get; set; }

    [JsonIgnore]
    public virtual IEnumerable<StoreAttachment>? StoreAttachmentsNavigation { set; get; }
}
