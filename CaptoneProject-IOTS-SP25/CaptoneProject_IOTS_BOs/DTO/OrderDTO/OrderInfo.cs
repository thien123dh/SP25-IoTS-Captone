using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.OrderDTO
{
    public class OrderInfo
    {
        [MaxLength(500)]
        public string Address { set; get; } = "";

        [MaxLength(150)]
        public string ContactNumber { set; get; } = "";

        [MaxLength(300)]
        public string? Notes { set; get; } = "";
    }
}
