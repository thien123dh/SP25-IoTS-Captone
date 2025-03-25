using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Message
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public int? CreatedBy { get; set; }

        public int? ReceiverId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }
    }
}
