using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public class ActivityLog
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public string Content { set; get; }
        public int? UserId { set; get; }
        public DateTime? CreatedDate { set; get; } = DateTime.Now;
    }
}
