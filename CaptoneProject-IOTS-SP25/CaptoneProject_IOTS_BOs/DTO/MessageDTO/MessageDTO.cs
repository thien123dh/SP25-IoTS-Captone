using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.MessageDTO
{
    public class CreateMessageDTO
    {
        public int ReceiverId { get; set; }
        public string Content { get; set; }
    }

    public class MessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int? CreatedBy { get; set; }
        public int? ReceiverId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
