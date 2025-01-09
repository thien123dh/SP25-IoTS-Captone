using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.UserRequestDTO
{
    public class VerifyOTPRequestDTO
    {
        public string OTP { set; get; }
        public string Email { set; get; }
    }
}
