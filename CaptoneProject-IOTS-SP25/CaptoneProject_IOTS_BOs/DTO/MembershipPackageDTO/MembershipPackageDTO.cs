using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.MembershipPackageDTO
{
    public class CreateUpdateAccountMembershipPackage
    {
        public int UserId { set; get; }
        public int MembershipPackageId { set; get; }
    }

    public class AccountMembershipPackageDTO
    {
        public int UserId { set; get; }

        public MembershipPackage MembershipPackageTypeNavigation { set; get; }

        public DateTime LastPaymentDate { set; get; }

        public DateTime NextPaymentDate { set; get; }
    }

}
