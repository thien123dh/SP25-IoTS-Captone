using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Combo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [MaxLength(300)]
        public string Name { set; get; } = "";

        public int Quantity { set; get; }

        [ForeignKey(nameof(Store))]
        public int StoreId { set; get; }
        
        [ForeignKey("StoreId")]
        public Store? StoreNavigation { set; get; }

        [MaxLength(300)]
        [Required]
        public string Summary { set; get; }

        [MaxLength(1000)]
        public string? Description { set; get; }

        [MaxLength(500)]
        public string? Specifications { set; get; }

        [MaxLength(500)]
        public string? Notes { set; get; }

        [MaxLength(100)]
        public string? SerialNumber { set; get; }

        [MaxLength(150)]
        public string? ApplicationSerialNumber { set; get; }

        [MaxLength(1000)]
        public string? ImageUrl { set; get; }

        [Precision(10, 1)]
        public decimal Weight { set; get; } = 0;

        public decimal Price { set; get; }

        public DateTime CreatedDate { set; get; } = DateTime.Now;

        public DateTime UpdateDate { set; get; } = DateTime.Now;

        [ForeignKey(nameof(User))]
        public int? CreatedBy { set; get; }

        [ForeignKey(nameof(User))]
        public int? UpdatedBy { set; get; }

        [Range(0, 5)]

        [Precision(2, 1)]
        public decimal? Rating { set; get; } = 4;
        public int IsActive { set; get; } = 1;
    }
}
