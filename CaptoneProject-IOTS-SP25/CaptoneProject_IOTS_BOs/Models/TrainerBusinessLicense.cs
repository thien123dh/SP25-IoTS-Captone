using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace CaptoneProject_IOTS_BOs.Models
{
    [Table("TrainerBusinessLicense")]
    public partial class TrainerBusinessLicense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [Column("trainer_id")]
        [ForeignKey(nameof(User))]
        public int TrainerId { set; get; }
        //public virtual User TrainerNavigation { set; get; }

        [Column("front_identification")]
        [MaxLength(1000)]
        public string FrontIdentification { set; get; } = "";

        [Column("back_identification")]
        [MaxLength(1000)]
        public string BackIdentification { set; get; } = "";

        [Column("business_listenses")]
        [MaxLength(1000)]
        public string BusinessLicences { set; get; } = "";

        [Column("issue_date")]
        public DateTime IssueDate { set; get; } = DateTime.Now;

        [Column("expired_date")]
        public DateTime ExpiredDate { set; get; } = DateTime.Now;

        [Column("issue_by")]
        [MaxLength(255)]
        public string IssueBy { set; get; } = "";

       
    }
}
