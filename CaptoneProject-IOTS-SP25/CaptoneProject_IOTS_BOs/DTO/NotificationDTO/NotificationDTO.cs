using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.NotificationDTO
{
    public class NotificationRequestDTO
    {
        public string? Title { set; get; }

        public string? Content { set; get; }

        public int EntityId { set; get; }

        public int EntityType { set; get; }

        public int ReceiverId { set; get; }
    }
}
