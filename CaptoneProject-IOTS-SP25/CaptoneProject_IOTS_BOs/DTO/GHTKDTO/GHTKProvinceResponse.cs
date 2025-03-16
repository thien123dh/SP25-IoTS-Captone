using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.GHTKDTO
{
    public class GHTKProvinceResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public List<GHTKProvinceData> Data { get; set; }
    }

    public class GHTKProvinceData
    {
        [JsonProperty("PROVINCE_ID")]
        public int PROVINCE_ID { get; set; }

        [JsonProperty("PROVINCE_NAME")]
        public string PROVINCE_NAME { get; set; }
    }
}
