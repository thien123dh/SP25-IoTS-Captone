using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class MembershipPackage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [Column("label")]
        public string Label { set; get; }

        [Column("number_of_month")]
        [JsonIgnore]
        public int NumberOfMonth { set; get; }

        [Column("fee")]
        [Precision(18, 1)]
        public decimal? Fee { set; get; }

        [Column("is_active")]
        [Range(0, 5)]
        [JsonIgnore]
        public int IsActive { set; get; }
    }
}
