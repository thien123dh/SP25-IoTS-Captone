using CaptoneProject_IOTS_BOs.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);

        EmailTemplate GetStaffManagerOtpEmailTemplate(string otp, string redirectUrl, string To);
    }
}
