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
        public static EmailTemplate CreateStaffOrManagerEmailTemplate(string ToEmail, string otp) => new EmailTemplate
        {
            Subject = "Verify Account",
            Body = $"Dear {ToEmail},\n\n" +
            $"Test.\n\n" +
                       $"OTP: {otp}\n\n" +
                       $"Best regards,\nThe Admin Team"
        };
    }
}
