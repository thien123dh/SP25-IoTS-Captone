using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [ForeignKey(nameof(IotsDevice))]
        public int? IosDeviceId { set; get; }

        [ForeignKey(nameof(Combo))]
        public int? ComboId { set; get; }

        [ForeignKey(nameof(Lab))]
        public int? LabId { set; get; }

        [ForeignKey(nameof(User))]
        public int SellerId { set; get; }

        [Range(1, 10)]
        [Required]
        public int ProductType { set; get; }

        [ForeignKey(nameof(CartItem))]
        public int? ParentCartItemId { set; get; }

        public int Quantity { set; get; } = 0;

        public int CreatedBy { set; get; }

        public DateTime UpdatedDate { set; get; } = DateTime.Now;

        public bool IsSelected { set; get; } = false;
    }
}
