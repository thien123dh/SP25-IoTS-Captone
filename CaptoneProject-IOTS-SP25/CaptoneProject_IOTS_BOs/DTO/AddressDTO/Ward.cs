using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.AddressDTO
{
    public class Ward
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DistrictId { get; set; }
    }

    public class ApiWard
    {
        public int code { get; set; }
        public string name { get; set; }
    }
}
