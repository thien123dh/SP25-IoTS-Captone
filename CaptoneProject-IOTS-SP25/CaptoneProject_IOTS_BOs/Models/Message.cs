using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Message
    {
        public int Id { get; set; }

        public string Content { get; set; }

        [ForeignKey(nameof(User))]
        public int? CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public virtual User CreatedByNavigation { set; get; }

        [ForeignKey(nameof(User))]
        public int? ReceiverId { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public virtual User Receiver { set; get; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }
    }
}
