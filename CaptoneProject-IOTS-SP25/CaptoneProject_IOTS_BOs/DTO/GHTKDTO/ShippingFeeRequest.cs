using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.GHTKDTO
{
    public class ShippingFeeRequest
    {
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public int AddressId { get; set; }
        public string Address { get; set; }

        [Required]
        [RegularExpression("^(xteam|none)$", ErrorMessage = "deliver_option chỉ được phép là 'xteam' hoặc 'none'")]
        public string deliver_option { get; set; }
    }

    public class ShippingFeeRequestByMobile
    {
        public int UserId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public int AddressId { get; set; }
        public string Address { get; set; }

        [Required]
        [RegularExpression("^(xteam|none)$", ErrorMessage = "deliver_option chỉ được phép là 'xteam' hoặc 'none'")]
        public string deliver_option { get; set; }
    }
}
