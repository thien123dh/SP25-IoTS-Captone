using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaptoneProject_IOTS_BOs.Models;

public partial class Material
{
    public int Id { get; set; }

    public string Name { get; set; }

    [MaxLength(500)]
    [Column("summary")]
    public string Summary { set; get; }
    public string Description { get; set; }

    public int? CategoryId { get; set; }

    public string Manufacturer { get; set; }

    public string Model { get; set; }

    public string SerialNumber { get; set; }

    public int? FirmwareVersion { get; set; }

    public string Connectivity { get; set; }

    public string PowerSource { get; set; }

    public DateTime? LastWarrentyDate { get; set; } = DateTime.Now.AddDays(30);

    [Column("store_id")]
    [ForeignKey(nameof(Store))]
    public int StoreId { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; } = DateTime.Now;

    public int? UpdatedBy { get; set; }

    public int? Quantity { get; set; } = 0;

    public decimal? Price { get; set; } = 0;

    public int? IsActive { get; set; } = 1;

    [MaxLength(1000)]
    [Column("image_url")]
    public string? ImageUrl { set; get; }

    public virtual Store StoreNavigation { get; set; }
    public MaterialCategory Category { get; set; }

    public virtual User CreatedByNavigation { get; set; }

    public virtual ICollection<MaterialGroupItem> MaterialGroupItems { get; set; } = new List<MaterialGroupItem>();

    public virtual User UpdatedByNavigation { get; set; }
}
