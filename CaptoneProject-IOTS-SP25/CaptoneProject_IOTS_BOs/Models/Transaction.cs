using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [ForeignKey(nameof(User))]
        public int UserId { set; get; }

        [Precision(18, 1)]
        public decimal Amount { set; get; }

        public string TransactionType { set; get; }

        public string Status { set; get; }

        public DateTime CreatedDate { set; get; } = DateTime.UtcNow.AddHours(7);

        [MaxLength(250)]
        public string Description { set; get; }

        public decimal? CurrentBallance { set; get; }
        [JsonIgnore]
        public virtual User UserNavigation { set; get; }

        public short IsApplication { set; get; } = 0;
    }
}
