using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.MaterialGroupCategoryDTO
{
    public class CreateUpdateMaterialGroupCategoryDTO
    {
        [JsonRequired]
        public string Label { get; set; }
    }
}
