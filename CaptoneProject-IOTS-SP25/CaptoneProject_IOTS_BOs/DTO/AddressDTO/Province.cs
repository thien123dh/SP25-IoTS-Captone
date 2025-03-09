using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.AddressDTO
{
    public class Province
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ApiProvince
    {
        public int code { get; set; }
        public string name { get; set; }
        public List<ApiDistrict> districts { get; set; }
    }
}
