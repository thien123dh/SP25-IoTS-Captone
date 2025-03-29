using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.RabbitMQDTO
{
    public class ChatAccountDTO
    {
    }

    public class RecentChatDTO
    {
        public int? UserId { get; set; }
        public string? Username { get; set; }

        [MaxLength(1000)]
        [Column("image_url")]
        public string? ImageURL { set; get; }
        public string LastMessage { get; set; } 
        public DateTime LastMessageTime { get; set; } 
    }
}
