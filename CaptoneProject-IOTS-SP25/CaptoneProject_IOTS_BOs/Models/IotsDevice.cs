using Azure;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CaptoneProject_IOTS_BOs.Models;

[Table("IotsDevices")]
public partial class IotsDevice
{
    public int Id { get; set; }

    public string Name { get; set; }

    [Column("device_type")]
    public int DeviceType { set; get; } = 1;

    [Column("secondhand_price")]
    public decimal? SecondHandPrice { set; get; } = 0;

    [Column("secondhand_quality_percent")]
    [Range(0, 100)]
    public int? SecondhandQualityPercent { set; get; } = 0;

    [MaxLength(500)]
    [Column("summary")]
    public string Summary { set; get; }
    public string Description { get; set; }
    [Precision(10, 1)]
    public decimal Weight { set; get; } = 0;
    [ForeignKey(nameof(MaterialCategory))]
    public int CategoryId { get; set; }

    public string Manufacturer { get; set; }

    public string Model { get; set; }

    public string SerialNumber { get; set; }

    public string ApplicationSerialNumber { set; get; }

    [Column("specifications")]
    [MaxLength(500)]
    public string Specifications { set; get; }

    [Column("notes")]
    [MaxLength(500)]
    public string Notes { set; get; }

    [Column("price")]
    [Precision(18, 1)]
    public decimal Price { set; get; } = 0;

    [Column("quantity")]
    public int Quantity { set; get; } = 0;

    [Column("store_id")]
    [ForeignKey(nameof(Store))]
    public int StoreId { get; set; }
    public DateTime? CreatedDate { get; set; } = DateTime.Now;

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; } = DateTime.Now;

    public int? UpdatedBy { get; set; }

    public int? IsActive { get; set; } = 1;

    [Range(0, 5)]

    [Precision(2, 1)]
    public decimal? Rating { set; get; } = 4;
    [MaxLength(1000)]
    [Column("image_url")]
    public string? ImageUrl { set; get; }

    public virtual Store StoreNavigation { get; set; }
    public virtual MaterialCategory Category { get; set; }

    public virtual User CreatedByNavigation { get; set; }

    public virtual User UpdatedByNavigation { get; set; }

    [JsonIgnore]
    public virtual IEnumerable<DeviceSpecification>? DeviceSpecifications { set; get; }
}
