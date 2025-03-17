using Newtonsoft.Json;
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
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parent_id")]
        public int? ParentId { get; set; } 
    }
}
