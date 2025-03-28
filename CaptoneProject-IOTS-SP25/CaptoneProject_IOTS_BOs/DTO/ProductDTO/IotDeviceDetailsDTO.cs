using CaptoneProject_IOTS_BOs.DTO.AttachmentDTO;
using CaptoneProject_IOTS_BOs.Models;
using System.ComponentModel.DataAnnotations;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_BOs.DTO.MaterialDTO
{
    public class DeviceSpecificationDTO
    {
        //public int Id { set; get; }

        public string Name { set; get; } = "";

        [Required]
        public List<DeviceSpecificationItemDTO>? DeviceSpecificationItemsList { set; get; }
    }

    public class DeviceSpecificationItemDTO
    {
        //public int Id { set; get; }

        public string? SpecificationProperty { set; get; }

        public string? SpecificationValue { set; get; }
    }

    public class CreateUpdateIotDeviceDTO
    {
        public string Name { get; set; }
        public IotDeviceTypeEnum DeviceType { set; get; }
        public decimal Weight { set; get; }
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
        public int WarrantyMonth { set; get; } = 0;

        public List<AttachmentsModelDTO>? Attachments { set; get; }
        public List<DeviceSpecificationDTO>? DeviceSpecificationsList { set; get; }
    }
    public class IotDeviceDetailsDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int DeviceType { set; get; } = 1;
        public string Summary { set; get; }
        public string Description { get; set; }

        public decimal? Weight { set; get; }
        public int? CategoryId { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string SerialNumber { get; set; }

        public string ApplicationSerialNumber { set; get; }

        public string Specifications { set; get; }

        public string Notes { set; get; }

        public decimal Rating { set; get; } = 5;
        public decimal Price { set; get; } = 0;

        public int Quantity { set; get; } = 0;
        public int StoreId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }

        public int? IsActive { get; set; } = 1;
        public string? ImageUrl { set; get; }
        public int WarrantyMonth { set; get; } = 0;
        public MaterialCategory? Category { set; get; }
        public StoreDetailsResponseDTO? StoreInfo { set; get; }
        public IEnumerable<AttachmentsModelDTO>? Attachments { set; get; }
        public IEnumerable<DeviceSpecificationDTO>? DeviceSpecificationsList { set; get; }
        public decimal? SecondHandPrice { set; get; }
        public int? SecondhandQualityPercent { set; get; }

        public bool IsEdit { set; get; }
    }

    public class IotDeviceDetailsUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? IsActive {  set; get; }
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

        public int? UpdatedBy { get; set; }

    }

    public class IotDeviceItem
    {
        public int Id { set; get; }
        public int DeviceType { set; get; }
        public string? DeviceTypeLabel { set; get; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public int StoreId { set; get; }
        public int CategoryId { set; get; }
        public string? CategoryName { set; get; }
        public string? StoreNavigationName { set; get; }
        public string? StoreNavigationImageUrl { set; get; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { set; get; }
        public int? IsActive { set; get; }
        public decimal? SecondHandPrice { set; get; }
        public int? SecondhandQualityPercent { set; get; }
        public decimal? Rating { set; get; }

        public int WarrantyMonth { set; get; }
    }

    public class IotDeviceByStoreDetailsResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public decimal? Rating { set; get; } = 4;
        public DateTime? CreatedDate { get; set; }
    }
}
