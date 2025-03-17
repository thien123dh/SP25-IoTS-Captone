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
        public string Address { set; get; }
        public string ContactNumber { set; get; }
        public string? Notes { set; get; }
        public int ProvinceId { set; get; }
        public int DistrictId { set; get; }
        public int WardId { set; get; }
        public int AddressId { set; get; }

    }
}
