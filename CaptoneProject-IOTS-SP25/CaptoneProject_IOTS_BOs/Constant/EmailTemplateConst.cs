using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public class EmailTemplate
    {
        public string Subject { set; get; }
        public string Body { set; get; }
    }

    public static class EmailTemplateConst
    {
        public static EmailTemplate CreateStaffOrManagerEmailTemplate(string To, string otp, string redirectUrl)
        {

            string htmlTemplate = File.ReadAllText("staff_manager_verify_otp_template.html");

            string emailBody = htmlTemplate
                .Replace("{OTP}", otp)
                .Replace("{link}", redirectUrl)
                .Replace("{Receiver}", To);

            return new EmailTemplate
            {
                Subject = "Verify OTP",

                Body = emailBody
            };
        }
    }
}
