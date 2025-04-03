using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_BOs.DTO.WarrantyRequestDTO
{
    public class WarrantyRequestResponseDTO
    {
        public int Id { set; get; }
        public string Description { set; get; }
        public int OrderItemId { set; get; }

        public int OrderId { set; get; }
        public int ProductId { set; get; }

        public int ProductType { set; get; }

        public string ProductName { set; get; }
        public string Remarks { set; get; }
        public int Status { set; get; }
        public string IdentifySerialNumber { set; get; }
        public string VideoUrl { set; get; }
        public string ContactNumber { set; get; }
        public int CreatedBy { set; get; }
        public string? CreatedByEmail { set; get; }

        public string? CreatedByFullName { set; get; }
        public DateTime CreatedDate { set; get; }
        public DateTime ActionDate { set; get; }
        public StoreDetailsResponseDTO? StoreInfo { set; get; }
    }

    public class WarrantyRequestRequestDTO
    {
        public string Description { set; get; }

        public int OrderItemId { set; get; }

        public string Remarks { set; get; }

        public string IdentifySerialNumber { set; get; }

        public string ContactNumber { set; get; }

        public string VideoUrl { set; get; }
    }
}
