using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.AttachmentDTO
{
    public class AttachmentsModelDTO
    {
        public int Id { set; get; }
        public string? ImageUrl { set; get; }
        public string? MetaData { set; get; }
        [JsonIgnore]
        public DateTime? CreatedDate { set; get; } = DateTime.Now;
        [JsonIgnore]
        int? CreatedBy { set; get; }
    }
}
