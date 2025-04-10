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
        private string verifyOtpTemplate = "<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <style>\r\n        body { font-family: Arial, sans-serif; }\r\n        .container { padding: 20px; background-color: #f4f4f4; }\r\n        .content { background: white; padding: 20px; border-radius: 10px; }\r\n        .footer { font-size: 12px; color: #777; margin-top: 20px; }\r\n    </style>\r\n</head>\r\n<body>\r\n    <table cellpadding=\"0\" cellspacing=\"0\" align=\"center\" width=\"100%\" style=\"background-color:#f7f7f7; font-family:Arial, Helvetica, sans-serif\">\r\n        <tr>\r\n            <td valign=\"top\">\r\n                <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" align=\"center\" style=\"background:rgba(0,0,0,0.1);margin-top:20px;\">\r\n                    <tr>\r\n                        <td width=\"18\"></td>\r\n                        <td width=\"95%\" style=\"background-color:#ffffff; padding:15px\" valign=\"top\">\r\n                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n                                <tr>\r\n                                    <td align=\"center\" valign=\"top\"><a href=\"[_site_url]\">\r\n                                        <img style=\"height: 300px; width: 100%\" src='https://fe-capstone-io-ts.vercel.app/assets/backgroundLogin-DgmFQXil.png' />\r\n                                    </a></td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"background:#9acb51;color:#ffffff; font-size:18px; text-transform:capitalize; font-family:sans-serif; font-weight:bold; padding:10px;\">\r\n                                        Dear {Receiver},\r\n                                    </td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td>&nbsp;</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"color:#999999; font-weight:bold; font-size:16px;\">Welcome to E-Iots Website</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"color:#999999; font-size:16px; padding:15px 0px;\">Here is your OTP</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"color:#999999; font-size:16px; font-weight:bold\">{OTP}</td>\r\n                                </tr>\r\n                            </table>\r\n                            <br>\r\n                            <a href=\"{link}\">Please click here to login</a>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n</body>\r\n</html>\r\n";
        private string invoiceEmailTemplate = "<!DOCTYPE html><html lang='en'><head><meta charset='UTF-8'><meta name='viewport' content='width=device-width, initial-scale=1.0'><title>Invoice</title><style>body { font-family: 'Arial', sans-serif; background: #f8f9fa; padding: 20px; display: flex; justify-content: center; align-items: center; min-height: 100vh; background-image: url('https://i.imgur.com/YOUR_IMAGE.jpg'); background-size: cover; background-position: center; } .invoice-container { width: 700px; background: white; padding: 20px; box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1); border: 2px solid #555; position: relative; opacity: 0.95; } .header, .invoice-date { text-align: center; } .divider { border-bottom: 1px solid #555; margin: 10px 0; } .info-box p { margin: 5px 0; font-size: 16px; } .table-container { width: 100%; border-collapse: collapse; margin-top: 10px; } .table-container th, .table-container td { border-bottom: 1px solid #ddd; padding: 10px; text-align: left; } .table-container th { background: white; font-size: 16px; font-weight: bold; } .table-container img { width: 50px; height: auto; display: block; } .total-row td { font-weight: bold; background: #f1f1f1; text-align: right; } .total-amount { text-align: right; font-size: 18px; font-weight: bold; margin-top: 10px; }</style></head><body><div class='invoice-container'><table align='center' width='100%'><tr><td align='center'><table align='center'><tr><td style='padding: 10px;'><img src='https://icharm.vn/wp-content/uploads/2023/10/ghtk.png' alt='Giao Hàng Tiết Kiệm' width='100'></td><td style='padding: 10px;'><img src='https://i.imgur.com/8IC5op2.jpeg' alt='Company Logo' width='100'></td><td style='padding: 10px;'><img src='https://vinadesign.vn/uploads/images/2023/05/vnpay-logo-vinadesign-25-12-57-55.jpg' alt='VNPAY' width='100'></td></tr></table></td></tr></table><div class='header'>Invoice Information</div><div class='invoice-date'>Date: {DATE}</div><div class='divider'></div><div class='info-box'><p><strong>Invoice ID:</strong> {INVOICE_ID}</p></div><div class='divider'></div><div class='info-box'><p><strong>Sender:</strong> {SENDER_NAME}</p><p>{SENDER_ADDRESS}</p><p>Email: {SENDER_EMAIL}</p></div><div class='divider'></div><div class='info-box'><p><strong>Recipient:</strong> {RECEIVER_NAME}</p><p>{RECEIVER_PROVINCE}, {RECEIVER_DISTRICT}, {RECEIVER_WARD}, {RECEIVER_ADDRESS}</p></div><div class='divider'></div><table class='table-container'><thead><tr><th>Image</th><th>Product</th><th>Quantity</th><th>Unit Price</th><th>Total</th></tr></thead><tbody>{PRODUCT_ROWS}<tr class='total-row'><td colspan='4'>Total Product Price:</td><td>{TOTAL_PRODUCT_PRICE}₫</td></tr><tr class='total-row'><td colspan='4'>Shipping Fee:</td><td>{SHIPPING_FEE}₫</td></tr></tbody></table><div class='divider'></div><p class='total-amount'>Total Amount: {TOTAL_AMOUNT}₫</p></div></body></html>";
        private string staffManagerTemplate = "<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <style>\r\n        body { font-family: Arial, sans-serif; }\r\n        .container { padding: 20px; background-color: #f4f4f4; }\r\n        .content { background: white; padding: 20px; border-radius: 10px; }\r\n        .footer { font-size: 12px; color: #777; margin-top: 20px; }\r\n    </style>\r\n</head>\r\n<body>\r\n    <table cellpadding=\"0\" cellspacing=\"0\" align=\"center\" width=\"100%\" style=\"background-color:#f7f7f7; font-family:Arial, Helvetica, sans-serif\">\r\n        <tr>\r\n            <td valign=\"top\">\r\n                <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" align=\"center\" style=\"background:rgba(0,0,0,0.1);margin-top:20px;\">\r\n                    <tr>\r\n                        <td width=\"18\"></td>\r\n                        <td width=\"95%\" style=\"background-color:#ffffff; padding:15px\" valign=\"top\">\r\n                            <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n                                <tr>\r\n                                    <td align=\"center\" valign=\"top\"><a href=\"[_site_url]\">\r\n                                        <img style=\"height: 300px; width: 100%\" src='https://fe-capstone-io-ts.vercel.app/assets/backgroundLogin-DgmFQXil.png' />\r\n                                    </a></td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"background:#9acb51;color:#ffffff; font-size:18px; text-transform:capitalize; font-family:sans-serif; font-weight:bold; padding:10px;\">\r\n                                        Dear {Receiver},\r\n                                    </td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td>&nbsp;</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"color:#999999; font-weight:bold; font-size:16px;\">Welcome to E-Iots Website</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"color:#999999; font-size:16px; padding:15px 0px;\">Here is your Password</td>\r\n                                </tr>\r\n                                <tr>\r\n                                    <td style=\"color:#999999; font-size:16px; font-weight:bold\">{Password}</td>\r\n                                </tr>\r\n                            </table>\r\n                            <br>\r\n                            <a href=\"{link}\">Please click here to login</a>\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n</body>\r\n</html>\r\n";

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public EmailTemplate GetVerifyOtpTemplate(string otp, string redirectUrl, string To)
        {
            //string htmlTemplate = File.ReadAllText("staff_manager_verify_otp_template.html");

            string htmlTemplate = verifyOtpTemplate;

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
            email.Cc.AddRange(new[]
            {
                MailboxAddress.Parse("thien0914033912@gmail.com"),
                MailboxAddress.Parse("Thinhncse160927@fpt.edu.vn"),
                MailboxAddress.Parse("vietleruoi@gmail.com")
            });
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

        public EmailTemplate GetStaffManagerTemplate(string password, string redirectUrl, string To)
        {
            string htmlTemplate = staffManagerTemplate;

            string emailBody = htmlTemplate
                .Replace("{Password}", password)
                .Replace("{link}", redirectUrl)
                .Replace("{Receiver}", To);

            return new EmailTemplate
            {
                Subject = "Your Account Has Been Created",

                Body = emailBody
            };
        }

        public async Task SendInvoiceEmailAsync(string to, string invoiceId, string senderName, string senderAddress, string senderEmail,
                                        string receiverName, string receiverProvince, string receiverDictricts, string receiverWards, string receiverAddress, List<ProductBillDTO> products,decimal totalProductPrice, decimal totalFee,decimal totalAmount)
        {
            string htmlBody = invoiceEmailTemplate
                .Replace("{DATE}", DateTime.Now.ToString("dd/MM/yyyy"))
                .Replace("{INVOICE_ID}", invoiceId)
                .Replace("{SENDER_NAME}", senderName)
                .Replace("{RECEIVER_PROVINCE}", receiverProvince)
                .Replace("{RECEIVER_DISTRICT}", receiverDictricts)
                .Replace("{RECEIVER_WARD}", receiverWards)
                .Replace("{SENDER_ADDRESS}", senderAddress)
                .Replace("{SENDER_EMAIL}", senderEmail)
                .Replace("{RECEIVER_NAME}", receiverName)
                .Replace("{RECEIVER_ADDRESS}", receiverAddress)
                .Replace("{SHIPPING_FEE}", totalFee.ToString("N0"))
                .Replace("{TOTAL_PRODUCT_PRICE}", totalProductPrice.ToString("N0"))
                .Replace("{TOTAL_AMOUNT}", totalAmount.ToString("N0"));


            // Xây dựng danh sách sản phẩm
            string productRows = "";
            foreach (var product in products)
            {
                productRows += $"<tr><td><img src='{product.Img}' alt='Ảnh sản phẩm'></td><td>{product.Name}</td><td>{product.Quantity}</td><td>{product.Price.ToString("N0")}₫</td><td>{(product.Price * product.Quantity).ToString("N0")}₫</td></tr>";
            }
            htmlBody = htmlBody.Replace("{PRODUCT_ROWS}", productRows);

            // Tạo email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSettings:From"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Cc.AddRange(new[]
            {
                MailboxAddress.Parse("thien0914033912@gmail.com"),
                MailboxAddress.Parse("Thinhncse160927@fpt.edu.vn"),
                MailboxAddress.Parse("vietleruoi@gmail.com")
            });

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
