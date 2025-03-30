using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [ForeignKey(nameof(Orders))]
        public int OrderId { set; get; }

        public virtual Orders Order { set; get; }

        [ForeignKey(nameof(IotsDevice))]
        public int? IosDeviceId { set; get; }
        public virtual IotsDevice IotsDevice { get; set; }

        [ForeignKey(nameof(Combo))]
        public int? ComboId { set; get; }
        public virtual Combo Combo { get; set; }

        [ForeignKey(nameof(Lab))]
        public int? LabId { set; get; }
        public virtual Lab Lab { get; set; }

        [ForeignKey(nameof(User))]
        public int SellerId { set; get; }
        public virtual User Seller { get; set; }

        [ForeignKey(nameof(User))]
        public int OrderBy { set; get; }

        public string? TxnRef { set; get; }

        public DateTime? UpdatedDate { set; get; }

        [Range(1, 10)]
        [Required]
        public int ProductType { set; get; }

        public string? TrackingId { set; get; }

        [Required]
        public int Quantity { set; get; } = 0;

        [Required]
        [Precision(18, 1)]
        public decimal Price { set; get; }

        public DateTime? WarrantyEndDate { set; get; } = null;

        [Range(0, 10)]
        public int OrderItemStatus { set; get; } = 1;
    }
}
