using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.StoreDTO
{
    public static class StoreDTO
    {
        public class StoreAttachmentResponseDTO
        {
            public int Id { get; set; }
            public int StoreId { set; get; }
            public DateTime? CreatedDate { get; set; }
            public int? CreatedBy { get; set; }
            public string? ImageUrl { set; get; }
        }
        public class StoreAttachmentRequestDTO
        {
            public int Id { get; set; }
            public string? ImageUrl { set; get; }
        }
        public class StoreRequestDTO
        {
            public string Name { get; set; }
            public string? ContactNumber { set; get; }
            public string? Address { set; get; }
            public string? Summary { set; get; }
            public string Description { get; set; }
            public string? ImageUrl { set; get; }
            public int ProvinceId { set; get; }
            public int DistrictId { set; get; }
            public int WardId { set; get; }

            public List<StoreAttachmentRequestDTO>? StoreAttachments { set; get; }
        }
        public class StoreResponseDTO
        {
            public int Id { set; get; }
            public string Name { get; set; }
            public string? Summary { set; get; }
            public int OwnerId { get; set; }
            public string OwnerName { set; get; }
            public int ProvinceId { set; get; }
            public int DistrictId { set; get; }
            public int WardId { set; get; }
            public DateTime? CreatedDate { set; get; }
            public int? CreatedBy { set; get; }
        }
        public class StoreDetailsResponseDTO
        {
            public int Id { set; get; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string? ContactNumber { set; get; }
            public string? Address { set; get; }
            public int ProvinceId { set; get; }
            public string ProvinceName { set; get; }
            public int DistrictId { set; get; }
            public string DistrictName { set; get; }
            public int WardId { set; get; }
            public string WardName { set; get; }
            public string? Summary { set; get; }
            public int OwnerId { get; set; }
            public string? ImageUrl { set; get; }
            public DateTime? CreatedDate { set; get; }
            public int? CreatedBy { set; get; }
            public DateTime? UpdatedDate { set; get; }
            public int? UpdatedBy { set; get; }
            public List<StoreAttachmentResponseDTO>? StoreAttachments { set; get; }
        }
    }
}
