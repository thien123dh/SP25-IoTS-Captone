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
            PENDING_TO_VERIFY_OTP = 4,
            VERIFIED_OTP = 5
            // STAFF/MANAGER ==> PENDING =(otp verify)=> APPROVED
            // CUSTOMER ===> PENDING =(otp verify)=> APPROVED
            
            //Store
            //Create request to verify email ==> pending to approve otp
            //Verified and registier user => VERIFIED_OTP
            //Submit store information => Pending to approved
            //Admin rejected => Rejected (the same with status 'verified otp')
            //Admin Approved => Approved and user status = 1 (active)
            
        }

    }
}
