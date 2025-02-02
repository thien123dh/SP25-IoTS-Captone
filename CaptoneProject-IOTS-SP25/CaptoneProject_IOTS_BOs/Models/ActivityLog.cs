using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    [Table("ActivityLog")]
    public class ActivityLog
    {
        [Key]
        public int Id { set; get; }
        public int EntityId { set; get; }
        public int EntityType { set; get; }
        public string Title { set; get; }
        public string Contents { set; get; }
        public string? Metadata { set; get; }
        public int? CreatedBy { set; get; }
        public DateTime? CreatedDate { set; get; }
    }
}
