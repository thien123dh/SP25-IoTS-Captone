using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.ProductRequestDTO
{
    public class ProductRequestDTO
    {
        public int Id { get; set; }

        public string? ProductName { set; get; }

        public string? Summary { set; get; }
        public int? ProductId { get; set; }
        public int? ProductType { get; set; }
        public string? Remark { get; set; }
        public int? ActionBy { get; set; }
        public string? ActionByEmail { set; get; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByEmail { set; get; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? Status { get; set; }  
    }
}
