using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [ForeignKey(nameof(Orders))]
        public int OrderId { set; get; }

        [ForeignKey(nameof(IotsDevice))]
        public int? IosDeviceId { set; get; }

        [ForeignKey(nameof(Combo))]
        public int? ComboId { set; get; }

        [ForeignKey(nameof(Lab))]
        public int? LabId { set; get; }

        [ForeignKey(nameof(User))]
        public int SellerId { set; get; }

        [ForeignKey(nameof(User))]
        public int OrderBy { set; get; }

        [Range(1, 10)]
        [Required]
        public int ProductType { set; get; }

        [Required]
        public int Quantity { set; get; } = 0;

        [Required]
        public int Price { set; get; }

        public DateTime? WarrantyEndDate { set; get; } = null;

        [Range(0, 10)]
        public int OrderItemStatus { set; get; } = 1;
    }
}
