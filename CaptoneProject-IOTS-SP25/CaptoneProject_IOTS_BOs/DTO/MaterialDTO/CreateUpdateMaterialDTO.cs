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

namespace CaptoneProject_IOTS_BOs.DTO.MaterialDTO
{
    public class CreateUpdateMaterialDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Summary { set; get; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int? CategoryId { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string SerialNumber { get; set; }

        public int? FirmwareVersion { get; set; }

        public string Connectivity { get; set; }

        public string PowerSource { get; set; }

        //public DateTime? LastWarrentyDate { get; set; }

        //public int? Quantity { get; set; }

        public decimal Price { get; set; }
        public string? ImageUrl { set; get; }

        public List<AttachmentsDTO> MaterialAttachments { set; get; }
    }

    public class MaterialItemDTO
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public MaterialCategory Category { get; set; }
        public Store? StoreNavigation { set; get; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { set; get; }
        public int? ProductStatus { set; get; }
    }

    public class MaterialDetailsResponseDTO
    {
        public int Id { set; get; }
        public string Name { get; set; }
        public string Summary { set; get; }

        public string Description { get; set; }

        public Store StoreNavigation { set; get; }

        public MaterialCategory Category { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string SerialNumber { get; set; }

        public int? FirmwareVersion { get; set; }

        public string Connectivity { get; set; }

        public string PowerSource { get; set; }

        //public DateTime? LastWarrentyDate { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { set; get; }
        public int? CreatedBy { set; get; }
        public int? UpdatedBy { set; get; }
        public DateTime CreatedDate { set; get; }
        public DateTime UpdatedDate { set; get; }
        public int? ProductStatus { set; get; }
        public List<AttachmentsDTO> MaterialAttachments { set; get; }
    }
}
