using CaptoneProject_IOTS_BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.OrderDTO
{
    public class OrderRequestDTO
    {
        [MaxLength(500)]
        public string Address { set; get; } = "";

        [MaxLength(150)]
        public string ContactNumber { set; get; } = "";

        [MaxLength(300)]
        public string? Notes { set; get; } = "";

        public int ProvinceId { set; get; }
        public int DistrictId { set; get; }
        public int WardId { set; get; }
        public int AddressId { set; get; }


        [Required]
        [RegularExpression("^(xteam|none)$", ErrorMessage = "deliver_option chỉ được phép là 'xteam' hoặc 'none'")]
        public string deliver_option { get; set; }
    }
}
