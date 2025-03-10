using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using Microsoft.EntityFrameworkCore;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Lab
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [MaxLength(150)]
        [Required]
        public string Title { set; get; }

        [MaxLength(300)]
        [Required]
        public string Summary { set; get; }

        [ForeignKey(nameof(Combo))]
        public int ComboId { set; get; }

        [ForeignKey(nameof(ComboId))]
        public virtual Combo? ComboNavigation { set; get; }

        [MaxLength(1000)]
        public string Description { set; get; }

        [MaxLength(300)]
        public string? Remark { set; get; }

        [MaxLength(100)]
        public string? SerialNumber { set; get; }

        [MaxLength(250)]
        public string ApplicationSerialNumber { set; get; }

        [MaxLength(1000)]
        public string ImageUrl { set; get; }

        [MaxLength(1000)]
        public string PreviewVideoUrl { set; get; }

        [Precision(18, 1)]
        public decimal Price { set; get; } = 0;

        public DateTime CreatedDate { set; get; } = DateTime.Now;

        public DateTime UpdatedDate { set; get; } = DateTime.Now;

        [ForeignKey(nameof(User))]
        public int? CreatedBy { set; get; }

        [ForeignKey("CreatedBy")]
        public virtual User CreatedByNavigation {set; get;}

        [ForeignKey(nameof(User))]
        public int? UpdatedBy { set; get; }

        [Range(0, 5)]
        [Precision(2, 1)]
        public decimal? Rating { set; get; } = 4;

        [Range(0, 10)]
        public int Status { set; get; } = (int)LabStatusEnum.DRAFT;
    }
}
