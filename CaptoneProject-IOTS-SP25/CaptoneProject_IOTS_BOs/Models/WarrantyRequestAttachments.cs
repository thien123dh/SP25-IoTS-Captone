using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public class WarrantyRequestAttachments
    {
        public int Id { set; get; }

        public string ImageUrl { set; get; }

        public string VideoUrl { set; get; }

        [ForeignKey(nameof(WarrantyRequest))]
        public int WarrantyRequestId { set; get; }

        [JsonIgnore]
        public string MetaData { set; get; } = "";
    }
}
