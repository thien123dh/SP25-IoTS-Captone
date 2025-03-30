using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
   public enum CashoutRequestStatusEnum
   {
        PENDING_TO_APPROVE = 0,
        APPROVED = 1,
        REJECTED = 2,
        INSUFFICIENT_WALLET_BALLANCE = 3
   }

}
