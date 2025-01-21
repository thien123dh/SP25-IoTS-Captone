using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.UserRequestDTO
{
    public class VerifyOTPRequestDTO
    {
        public string OTP { set; get; }
        [EmailAddress]
        public string Email { set; get; }
    }

    public class StaffManagerVerifyOtpRequest
    {
        public string OTP { set; get; }
        public int RequestId { set; get; }
        public string password { set; get; }
    }
}
