using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Wallet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [Column("ballance")]
        [Precision(18, 1)]
        public decimal Ballance { set; get; } = 0;

        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public int UserId { set; get; }

        [Column("created_date")]
        public DateTime CreatedDate = DateTime.Now;

        [Column("updated_date")]
        public DateTime UpdatedDate = DateTime.Now;
    }
}
