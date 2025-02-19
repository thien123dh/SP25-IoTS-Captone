using Azure;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public int? IsHardwareInformation { set; get; } = 1;
    //HARDWARE INFORMATION
    [Column("MCU_MPU")]
    [MaxLength(250)]
    public string? MCU_MPU { set; get; }

    [Column("memory")]
    [MaxLength(250)]
    public string? Memory { set; get; }

    [Column("wireless_connection")]
    [MaxLength(250)]
    public string? WirelessConnection { set; get; }

    public string? Connectivity { get; set; }

    [Column("sensor")]
    [MaxLength(250)]
    public string? Sensor { set; get; }
    //HARDWARE INFORMATION

    //Network Connection
    public int? IsNetworkConnection { set; get; } = 1;

    [Column("protocol")]
    [MaxLength(250)]
    public string? Protocol { set; get; }

    [Column("data_transmission_standard")]
    [MaxLength(250)]
    public string? DataTransmissionStandard { set; get; }

    [Column("network_security")]
    [MaxLength(250)]
    public string? NetworkSecurity { set; get; }
    //Network connection

    //Software or operations
    public int? IsSoftwareOrOperations { set; get; } = 1;

    [Column("firmware")]
    [MaxLength(250)]
    public string? Firmware { set; get; }

    public int? FirmwareVersion { get; set; }

    [Column("embedded_operating_system")]
    [MaxLength(250)]
    public string? EmbeddedEperatingSystem { set; get; }

    [Column("cloud_service")]
    [MaxLength(250)]
    public string? Cloudservice { set; get; }

    [Column("firmware_OTA_support")]
    public int FirmwareOTASupport { set; get; } = 1;
    //Software or operations

    //PowerSource
    public int? IsPowerSource { set; get; } = 1;

    [Column("operating_voltage")]
    [MaxLength(250)]
    public string? OperatingVoltage { set; get; }

    [Column("power_consumption")]
    [MaxLength(250)]
    public string? PowerConsumption { set; get; }

    public string? PowerSource { get; set; }
    //PowerSource

    //Security
    public int? IsSecurity { set; get; } = 1;
    [Column("data_encryption")]
    [MaxLength(250)]
    public string? DataEncryption { set; get; }

    [Column("device_authentication")]
    [MaxLength(250)]
    public string? DeviceAuthentication { set; get; }
    //Security

    //Performance
    [Column("connection_delay")]
    [MaxLength(250)]
    public string? ConnectionDelay { set; get; }

    [Column("processing_speed")]
    [MaxLength(250)]
    public string? ProcessingSpeed { set; get; }
    //Performance

    //Package
    [Column("service_life")]
    [MaxLength(250)]
    public string? ServiceLife { set; get; }

    [Column("durability")]
    [MaxLength(250)]
    public string? Durability { set; get; }
    //Package

    [MaxLength(500)]
    [Column("summary")]
    public string Summary { set; get; }
    public string Description { get; set; }

    public int? CategoryId { get; set; }

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

    [MaxLength(1000)]
    [Column("image_url")]
    public string? ImageUrl { set; get; }

    public virtual Store StoreNavigation { get; set; }
    public MaterialCategory Category { get; set; }

    public virtual User CreatedByNavigation { get; set; }

    public virtual User UpdatedByNavigation { get; set; }
}
