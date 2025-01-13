using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public static class UserRequestConstant
    {
        public enum UserRequestStatusEnum
        {
            PENDING_TO_APPROVE = 1,
            APPROVED = 2,
            REJECTED = 3,
            PENDING_TO_VERIFY_OTP = 4
            // STAFF/MANAGER ==> PENDING =(otp verify)=> APPROVED
            // CUSTOMER ===> PENDING =(otp verify)=> APPROVED

            //TODO: TRAINER/STORE ==> PENDING =(otp verify)=> PENDING TO APPROVE =(Manager/Admin Approve)=> APPROVED
            //                                                             =(Rejected)=> REJECTED
        }

    }
}
