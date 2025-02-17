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
    public class AccountMembershipPackage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public int UserId { set; get; }

        [Column("fee")]
        [Precision(18, 1)]
        public decimal Fee { set; get; }

        [Column("membership_package_type")]
        [ForeignKey(nameof(MembershipPackage))]
        public int MembershipPackageType { set; get; }

        public DateTime LastPaymentDate { set; get; }

        public DateTime NextPaymentDate { set; get; } = DateTime.Now;

        public virtual MembershipPackage MembershipPackageNavigation { set; get; }
    }
}
