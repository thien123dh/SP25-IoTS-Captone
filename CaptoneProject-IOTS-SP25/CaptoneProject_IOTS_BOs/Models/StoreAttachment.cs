using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class StoreAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { set; get; }
        [Column("store_id")]
        public int StoreId { set; get; }
        [Column("image_url")]
        [MaxLength(500)]
        public string? ImageUrl { set; get; }
        [Column("meta_data")]
        [MaxLength(500)]
        public string? MetaData { set; get; }
        [Column("created_date")]
        public DateTime CreatedDate { set; get; }
        [Column("created_by")]
        public int createdBy { set; get; }
        public virtual Store? StoreNavigation { set; get; }

    }
}
