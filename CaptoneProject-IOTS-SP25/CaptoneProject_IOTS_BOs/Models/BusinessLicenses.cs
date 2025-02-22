using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    [Table("business_licenses")]
    public partial class BusinessLicenses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [Column("store_id")]
        [ForeignKey(nameof(Store))]
        public int storeId { set; get; }

        [Column("front_identification")]
        [MaxLength(1000)]
        public string FrontIdentification { set; get; } = "";

        [Column("back_identification")]
        [MaxLength(1000)]
        public string BackIdentification { set; get; } = "";

        [Column("business_listenses")]
        [MaxLength(1000)]
        public string BusinessLicences { set; get; } = "";

        [Column("license_number")]
        [MaxLength(100)]
        public string LiscenseNumber { set; get; } = "";

        [Column("issue_date")]
        public DateTime IssueDate { set; get; } = DateTime.Now;

        [Column("expired_date")]
        public DateTime ExpiredDate { set; get; } = DateTime.Now;

        [Column("issue_by")]
        [MaxLength(255)]
        public string IssueBy { set; get; } = "";

        [JsonIgnore]
        public virtual Store StoreNavigation { set; get; }
    }
}
