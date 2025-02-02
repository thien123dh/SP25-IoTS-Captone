using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public class ActivityLog
    {
        public int EntityId { set; get; }
        public int EntityType { set; get; }
        public string Title { set; get; }
        public string Contents { set; get; }
        public string? MetaData { set; get; }
        public int? CreatedBy { set; get; }
        public DateTime? CreatedDate { set; get; }
    }
}
