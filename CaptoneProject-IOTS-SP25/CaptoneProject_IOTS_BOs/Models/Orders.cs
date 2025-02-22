using CaptoneProject_IOTS_BOs.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [MaxLength(200)]
        public string ApplicationSerialNumber { set; get; }

        [ForeignKey(nameof(User))]
        public int SellerId { set; get; }

        [ForeignKey(nameof(User))]
        public int OrderBy { set; get; }

        [MaxLength(300)]
        public string? Notes { set; get; } = "";

        [MaxLength(300)]
        public string? Remark { set; get; } = "";

        public DateTime CreateDate { set; get; } = DateTime.Now;

        [ForeignKey(nameof(User))]
        public int? CreatedBy { set; get; }

        public DateTime UpdatedDate { set; get; } = DateTime.Now;

        [ForeignKey(nameof(User))]
        public int? UpdatedBy { set; get; }

        [Range(1, 15)]
        public int OrderStatusId { set; get; }
    }
}
