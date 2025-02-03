using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CaptoneProject_IOTS_BOs.Models
{
    [Table("attachment")]
    public partial class Attachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { set; get; }
        [Column("entity_id")]
        public int EntityId { set; get; }
        [Column("entity_type")]
        public int EntityType { set; get; }
        [Column("image_url")]
        [MaxLength(1000)]
        public string? ImageUrl { set; get; }
        [Column("meta_data")]
        [MaxLength(500)]
        public string? MetaData { set; get; }
        [Column("created_date")]
        public DateTime? CreatedDate { set; get; } = DateTime.Now;
        [Column("created_by")]
        public int? CreatedBy { set; get; }
    }
}
