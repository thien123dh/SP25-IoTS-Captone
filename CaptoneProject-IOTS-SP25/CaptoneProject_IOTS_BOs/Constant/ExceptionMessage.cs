using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public static class ExceptionMessage
    {
        public static string USER_EXIST_EXCEPTION = "User is existing";

        public static string USER_DOESNT_EXIST = "User does not exist";

        public static string INVALID_STAFF_MANAGER_ROLE = "The role must be staff or manager";

        public static string USER_REQUEST_NOT_FOUND = "User request does not exist";

        public static string USER_EMAIL_INVALID = "User email is invalid";

        public static string EMAIL_ALREADY_VERIFIED = "Your email is already verified";

        public static string EXPIRED_OTP = "The otp was expired";

        public static string INCORRECT_OTP = "The entered otp was incorrect. Please try again";

        public static string LOGIN_INACTIVE_ACCOUNT = "Your account was inactive. Please contact with admin to activate";

        public static string MATERIAL_CATEGORY_NOTFOUND = "Cannot find material category";
    }
}
