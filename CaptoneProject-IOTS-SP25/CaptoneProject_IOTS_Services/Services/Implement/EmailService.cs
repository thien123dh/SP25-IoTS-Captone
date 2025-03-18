using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
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

        private string invoiceEmailTemplate = "<!DOCTYPE html><html lang='vi'><head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'><title>Hóa đơn điện tử</title><style>body { font-family: 'Arial', sans-serif; background: #f8f9fa; padding: 20px; display: flex; justify-content: center; align-items: center; min-height: 100vh; } .invoice-container { width: 700px; background: white; padding: 20px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1); border: 2px solid #555; position: relative; } .logo-container { display: flex; justify-content: space-between; align-items: center; } .logo-container img { width: 100px; } .header { text-align: center; font-size: 20px; font-weight: bold; margin-top: 10px; } .invoice-date { text-align: center; font-size: 14px; color: #555; margin-bottom: 10px; } .divider { border-bottom: 1px solid #555; margin: 10px 0; } .info-box p { margin: 5px 0; font-size: 16px; } .table-container { width: 100%; border-collapse: collapse; margin-top: 10px; } .table-container th, .table-container td { border-bottom: 1px solid #ddd; padding: 10px; text-align: left; } .table-container th { background: white; font-size: 16px; font-weight: bold; } .total { text-align: right; font-size: 18px; font-weight: bold; margin-top: 10px; } .pdf-link { text-align: center; margin-top: 10px; } .pdf-link a { background: #28a745; color: white; padding: 10px 20px; text-decoration: none; font-size: 16px; display: inline-block; }</style></head><body><div class='invoice-container'><div class='logo-container'><img src='./img/z6304340409977_5fc0c83e073ac8f4be46298fb28a3846.jpg' alt='Logo Công ty'></div><div class='header'>Thông Tin Hóa Đơn</div><div class='invoice-date'>Ngày tháng năm: {DATE}</div><div class='divider'></div><div class='info-box'><p><strong>Số hóa đơn:</strong> {INVOICE_ID}</p></div><div class='divider'></div><div class='info-box'><p><strong>Người gửi:</strong> {SENDER_NAME}</p><p>{SENDER_PROVINCE},{SENDER_DISTRICT},{SENDER_ADDRESS}</p><p>Email: {SENDER_EMAIL}</p></div><div class='divider'></div><div class='info-box'><p><strong>Người nhận:</strong> {RECEIVER_NAME}</p><p>{RECEIVER_ADDRESS}</p></div><div class='divider'></div><table class='table-container'><thead><tr><th>Sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr></thead><tbody>{PRODUCT_ROWS}</tbody></table><div class='divider'></div><p class='total'>Tổng tiền: {TOTAL_AMOUNT}₫</p></div></body></html>";

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

        public async Task SendInvoiceEmailAsync(string to, string invoiceId, string senderName, string senderAddress, string senderEmail,
                                        string receiverName, string receiverProvince, string receiverDictricts, string receiverAddress, List<ProductBillDTO> products, decimal totalAmount)
        {
            string htmlBody = invoiceEmailTemplate
                .Replace("{DATE}", DateTime.Now.ToString("dd/MM/yyyy"))
                .Replace("{INVOICE_ID}", invoiceId)
                .Replace("{SENDER_NAME}", senderName)
                .Replace("{SENDER_PROVINCE}", receiverProvince)
                .Replace("{SENDER_DISTRICT}", receiverDictricts)
                .Replace("{SENDER_ADDRESS}", senderAddress)
                .Replace("{SENDER_EMAIL}", senderEmail)
                .Replace("{RECEIVER_NAME}", receiverName)
                .Replace("{RECEIVER_ADDRESS}", receiverAddress)
                .Replace("{TOTAL_AMOUNT}", totalAmount.ToString("N0"));

            // Xây dựng danh sách sản phẩm
            string productRows = "";
            foreach (var product in products)
            {
                productRows += $"<tr><td>{product.Name}</td><td>{product.Quantity}</td><td>{product.Price.ToString("N0")}₫</td><td>{(product.Price * product.Quantity).ToString("N0")}₫</td></tr>";
            }
            htmlBody = htmlBody.Replace("{PRODUCT_ROWS}", productRows);

            // Tạo email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Cc.Add(MailboxAddress.Parse("thien0914033912@gmail.com"));
            email.Subject = "Hóa đơn điện tử từ " + senderName;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

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
                Console.WriteLine($"Lỗi gửi email: {ex.Message}");
            }
        }

    }
}
