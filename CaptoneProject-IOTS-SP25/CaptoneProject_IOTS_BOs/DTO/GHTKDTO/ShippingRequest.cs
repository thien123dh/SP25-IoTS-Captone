using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.GHTKDTO
{
    public class ShippingRequest
    {
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public int AddressId { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string note { get; set; }
    }

    public class ShippingRequestByMobile
    {
        public int UserId { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        public int AddressId { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string note { get; set; }
    }
}
