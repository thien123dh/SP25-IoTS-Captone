using CaptoneProject_IOTS_BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.DTO.AttachmentDTO;
using CaptoneProject_IOTS_BOs.Validation;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_BOs.DTO.ProductDTO
{
    public class ComboItemDTO
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public int StoreId { set; get; }

        public string? StoreNavigationName { set; get; }

        public string? Summary { set; get; }

        [MaxLength(150)]
        public string? ApplicationSerialNumber { set; get; }

        public int Quantity { set; get; }

        public string? ImageUrl { set; get; }

        public decimal Price { set; get; }

        public DateTime CreatedDate { set; get; }

        public DateTime UpdateDate { set; get; }

        public int? CreatedBy { set; get; }

        public int? UpdatedBy { set; get; }

        public decimal? Rating { set; get; }
        public int IsActive { set; get; }
    }

    public class DeviceComboReponseDTO
    {
        public int DeviceComboId { set; get; }
        public int IotDeviceId { set; get; }
        public string? DeviceName { set; get; }
        public string? DeviceSummary { set; get; }
        public int Amount { set; get; }
        public decimal OriginalPrice { set; get; }

        public string? ImageUrl { set; get; }
    }

    public class ComboByStoreDetailsResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public decimal? Rating { set; get; } = 4;
        public DateTime? CreatedDate { get; set; }
    }


    public class ComboDetailsResponseDTO
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public int StoreId { set; get; }

        public int Quantity { set; get; }

        public string? StoreNavigationName { set; get; }

        public string? Summary { set; get; }

        public string? Description { set; get; }

        public string? Specifications { set; get; }

        public string? Notes { set; get; }

        public string? SerialNumber { set; get; }

        public string? ApplicationSerialNumber { set; get; }

        public string? ImageUrl { set; get; }

        public decimal Price { set; get; }

        public decimal? Weight { set; get; }

        public DateTime CreatedDate { set; get; }

        public DateTime UpdateDate { set; get; }

        public int? CreatedBy { set; get; }

        public int? UpdatedBy { set; get; }

        public decimal? Rating { set; get; }

        public List<DeviceComboReponseDTO>? DeviceComboList { set; get; }

        public List<AttachmentsModelDTO>? AttachmentsList { set; get; }
        public StoreDetailsResponseDTO? StoreInfo { set; get; }
        public int IsActive { set; get; }
    }

    public class CreateUpdateDeviceComboDTO
    {
        public int DeviceComboId { set; get; }
        public int IotDeviceId { set; get; }
        public int Amount { set; get; }
    }
    public class CreateUpdateComboDTO
    {
        [Required]
        public string Name { set; get; }
        [Required]
        public int StoreId { set; get; }
        [Required]
        public string Summary { set; get; }
        [Required]
        public string? Description { set; get; }
        [Required]
        [PositiveInt(ErrorMessage = "Quantity cannot be negative")]
        
        public int Quantity { set; get; }

        [Required]
        public string? Specifications { set; get; }
        [Required]
        public string? Notes { set; get; }
        [Required]
        public string SerialNumber { set; get; } = "";
        [Required]
        public string? ImageUrl { set; get; }
        [Required]
        [PositiveDecimalAttribute(ErrorMessage = "Price cannot be negative")]
        public decimal Price { set; get; }

        public decimal Weight { set; get; } = 0;
        [Required]
        public List<CreateUpdateDeviceComboDTO> DeviceComboList { set; get; }
        [Required]
        public List<AttachmentsModelDTO> AttachmentsList { set; get; }
    }
    
}
