using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [Column("title")]
        [MaxLength(250)]
        public string? Title { set; get; }

        [Column("content")]
        [MaxLength(500)]
        public string? Content { set; get; }

        [Column("entity_id")]
        public int EntityId { set; get; }

        [Column("entity_type")]
        public int EntityType { set; get; }

        public DateTime? CreatedDate { set; get; } = DateTime.Now;

        [Column("receiver_id")]
        [ForeignKey(nameof(User))]
        public int ReceiverId { set; get; }

        [Column("metadata")]
        [MaxLength(500)]
        public string? Metadata { set; get; }

        public bool IsRead { set; get; } = false;

    }
}
