using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class LabAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [ForeignKey(nameof(Lab))]
        public int LabId { set; get; }

        [MaxLength(250)]
        public string Title { set; get; }

        [MaxLength(500)]
        public string Description { set; get; }

        [MaxLength(1000)]
        public string VideoUrl { set; get; }

        public int OrderIndex { set; get; }

        public DateTime CreatedDate { set; get; } = DateTime.Now;

        public DateTime UpdatedDate { set; get; } = DateTime.Now;
    }
}
