using CaptoneProject_IOTS_BOs.DTO.AttachmentDTO;
using CaptoneProject_IOTS_BOs.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.DTO.MaterialDTO.IotDeviceItem;

namespace CaptoneProject_IOTS_BOs.DTO.MaterialDTO
{
    public class CreateUpdateIotDeviceDTO
    {
        public string Name { get; set; }

        public IotDeviceTypeEnum DeviceType { set; get; }

        public int? IsHardwareInformation { set; get; } = 1;
        public string? MCU_MPU { set; get; }

        public string? Memory { set; get; }
        public string? WirelessConnection { set; get; }

        public string? Connectivity { get; set; }

        public string? Sensor { set; get; }
        //HARDWARE INFORMATION

        //Network Connection
        public int? IsNetworkConnection { set; get; } = 1;

        public string? Protocol { set; get; }

        public string? DataTransmissionStandard { set; get; }

        public string? NetworkSecurity { set; get; }
        //Network connection

        //Software or operations
        public int? IsSoftwareOrOperations { set; get; } = 1;

        public string? Firmware { set; get; }

        public int? FirmwareVersion { get; set; }

        public string? EmbeddedEperatingSystem { set; get; }

        public string? Cloudservice { set; get; }

        public int FirmwareOTASupport { set; get; } = 1;
        //Software or operations

        //PowerSource
        public int? IsPowerSource { set; get; } = 1;
        public string? OperatingVoltage { set; get; }

        public string? PowerConsumption { set; get; }

        public string? PowerSource { get; set; }
        //PowerSource

        //Security
        public int? IsSecurity { set; get; } = 1;
        public string? DataEncryption { set; get; }
        public string? DeviceAuthentication { set; get; }
        //Security

        //Performance
        public string? ConnectionDelay { set; get; }

        public string? ProcessingSpeed { set; get; }
        //Performance

        //Package
        public string? ServiceLife { set; get; }

        public string? Durability { set; get; }
        //Package
        public string Summary { set; get; }
        public string Description { get; set; }

        public int? CategoryId { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string SerialNumber { get; set; }

        public string Specifications { set; get; }

        public string Notes { set; get; }

        public decimal Price { set; get; } = 0;

        public int Quantity { set; get; } = 0;

        public int StoreId { get; set; }

        public string? ImageUrl { set; get; }

        public decimal? SecondHandPrice { set; get; } = 0;

        public int? SecondhandQualityPercent { set; get; } = 0;

        public List<AttachmentsModelDTO>? Attachments { set; get; }
    }
    public class IotDeviceDetailsDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int DeviceType { set; get; } = 1;

        public int? IsHardwareInformation { set; get; } = 1;
        public string? MCU_MPU { set; get; }

        public string? Memory { set; get; }
        public string? WirelessConnection { set; get; }

        public string? Connectivity { get; set; }

        public string? Sensor { set; get; }
        //HARDWARE INFORMATION

        //Network Connection
        public int? IsNetworkConnection { set; get; } = 1;

        public string? Protocol { set; get; }

        public string? DataTransmissionStandard { set; get; }

        public string? NetworkSecurity { set; get; }
        //Network connection

        //Software or operations
        public int? IsSoftwareOrOperations { set; get; } = 1;

        public string? Firmware { set; get; }

        public int? FirmwareVersion { get; set; }

        public string? EmbeddedEperatingSystem { set; get; }

        public string? Cloudservice { set; get; }

        public int FirmwareOTASupport { set; get; } = 1;
        //Software or operations

        //PowerSource
        public int? IsPowerSource { set; get; } = 1;
        public string? OperatingVoltage { set; get; }

        public string? PowerConsumption { set; get; }

        public string? PowerSource { get; set; }
        //PowerSource

        //Security
        public int? IsSecurity { set; get; } = 1;
        public string? DataEncryption { set; get; }
        public string? DeviceAuthentication { set; get; }
        //Security

        //Performance
        public string? ConnectionDelay { set; get; }

        public string? ProcessingSpeed { set; get; }
        //Performance

        //Package
        public string? ServiceLife { set; get; }

        public string? Durability { set; get; }
        //Package
        public string Summary { set; get; }
        public string Description { get; set; }

        public int? CategoryId { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string SerialNumber { get; set; }

        public string ApplicationSerialNumber { set; get; }

        public string Specifications { set; get; }

        public string Notes { set; get; }

        public decimal Price { set; get; } = 0;

        public int Quantity { set; get; } = 0;
        public int StoreId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }

        public int? IsActive { get; set; } = 1;
        public string? ImageUrl { set; get; }
        public MaterialCategory? Category { set; get; }
        public Store? StoreNavigation { set; get; }
        public IEnumerable<AttachmentsModelDTO>? Attachments { set; get; }
        public decimal? SecondHandPrice { set; get; }
        public int? SecondhandQualityPercent { set; get; }

        public bool IsEdit { set; get; }
    }

    public class IotDeviceItem
    {
        public int Id { set; get; }
        public int DeviceType { set; get; }
        public string? DeviceTypeLabel { set; get; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public MaterialCategory? Category { set; get; }
        public Store? StoreNavigation { set; get; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { set; get; }
        public int? IsActive { set; get; }
        public decimal? SecondHandPrice { set; get; }
        public int? SecondhandQualityPercent { set; get; }
    }
}
