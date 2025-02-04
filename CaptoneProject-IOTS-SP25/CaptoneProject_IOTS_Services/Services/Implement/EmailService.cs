using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_Service.Services.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private string staffManagerOtpEmailTemplate = "<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <style>\r\n        body { font-family: Arial, sans-serif; }\r\n        .container { padding: 20px; background-color: #f4f4f4; }\r\n        .content { background: white; padding: 20px; border-radius: 10px; }\r\n        .footer { font-size: 12px; color: #777; margin-top: 20px; }\r\n    </style>\r\n</head>\r\n<body>\r\n    <table cellpadding=\"0\" cellspacing=\"0\" align=\"center\" width=\"100%\" style=\"background-color:#f7f7f7; font-family:Arial, Helvetica, sans-serif\">\r\n        <tr>\r\n            <td valign=\"top\">\r\n                <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" align=\"center\" style=\"background:rgba(0,0,0,0.1);margin-top:20px;\">\r\n                    <tr>\r\n                        <td width=\"18\"></td>\r\n                        <td width=\"95%\" style=\"background-color:#ffffff; padding:15px\" valign=\"top\">\r\n                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n                                <tr>\r\n                                    <td style=\"height: 50%;\" align=\"center\" valign=\"top\">\r\n                                        <a href=\"[_site_url]\">\r\n                                            <img style=\"height: 300px; width: 100%\" src='https://www.cloudblue.com/wp-content/uploads/2024/06/what-is-the-internet-of-things-iot.png' />\r\n                                        </a>\r\n                                    </td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"background:#9acb51;color:#ffffff; font-size:18px; text-transform:capitalize; font-family:sans-serif; font-weight:bold; padding:10px;\">\r\n                                        Dear {Receiver},\r\n                                    </td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td>&nbsp;</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"color:#999999; font-weight:bold; font-size:16px;\">Welcome to E-Iots Website</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"color:#999999; font-size:16px; padding:15px 0px;\">Here is your OTP</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"color:#999999; font-size:16px; font-weight:bold\">{OTP}</td>\r\n                                </tr>\r\n                            </table>\r\n                            <br>\r\n                            <a href=\"{link}\">Please click here to verify OTP</a>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n</body>\r\n</html>\r\n";
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public EmailTemplate GetStaffManagerOtpEmailTemplate(string otp, string redirectUrl, string To)
        {
            //string htmlTemplate = File.ReadAllText("staff_manager_verify_otp_template.html");
            string htmlTemplate = staffManagerOtpEmailTemplate;

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

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // Create the email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Cc.Add(MailboxAddress.Parse("thien0914033912@gmail.com"));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    await smtpClient.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    await smtpClient.AuthenticateAsync("halvse140685@fpt.edu.vn", "dxxoydcscfapiypv");

                    await smtpClient.SendAsync(email);

                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
