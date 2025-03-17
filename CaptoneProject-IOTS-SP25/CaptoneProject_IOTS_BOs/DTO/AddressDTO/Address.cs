using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.AddressDTO
{
    public class Address
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WardId { get; set; }
    }

    public class ApiAddress
    {
        public int Id { get; set; }
        public string name { get; set; }
        public int parent_id { get; set; }
    }
}
