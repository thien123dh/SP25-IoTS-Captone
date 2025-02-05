using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO
{
    public class CreateUpdateMaterialCategoryDTO
    {
        [JsonRequired]
        public string Label { get; set; }
        public string Description { set; get; }
        public string? ImageUrl { set; get; }
    }
}
