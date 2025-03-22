using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
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

        Task SendInvoiceEmailAsync(string to, string invoiceId, string senderName, string senderAddress, string senderEmail,
                                        string receiverName, string receiverProvince, string receiverDictricts, string receiverWards, string receiverAddress, List<ProductBillDTO> products, decimal totalProductPrice, decimal totalFee, decimal totalAmount);
    }
}
