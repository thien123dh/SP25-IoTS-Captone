using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Business;
using CaptoneProject_IOTS_Service.Services.Interface;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class VNPayService : IVNPayService
    {
        private readonly IUserServices _userService;

        public VNPayService(IUserServices userService)
        {
            _userService = userService;
        }

        public async Task<string> CallAPIPayByUserId(int userId, string returnUrl, string paymentType)
        {
            try
            {
                string vnp_ReturnUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : "https://localhost:44346/checkout-process";
                string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string vnp_TmnCode = "2BF25S7Y";
                string vnp_HashSecret = "UWEORVE5ULXN8YNCLM16TFK1FWPQ0SA9";

                if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
                {
                    throw new Exception("Merchant code or secret key is missing.");
                }

                // Fix cứng số tiền theo loại thanh toán
                long amount;
                switch (paymentType.ToLower())
                {
                    case "short-term":
                        amount = 100000;
                        break;
                    case "normal":
                        amount = 500000;
                        break;
                    case "long-term":
                        amount = 1000000;
                        break;
                    default:
                        throw new Exception("Invalid payment type.");
                }
                var amounts = ((long)amount * 100).ToString();
                var vnp_TxnRef = $"{userId}{DateTime.Now:HHmmss}";
                var vnp_Amount = amounts;

                var vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", "2.1.0");
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", vnp_Amount);
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                vnpay.AddRequestData("vnp_CreateDate", vietnamTime.AddMinutes(-20).ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", $"Payment type: {paymentType}");
                vnpay.AddRequestData("vnp_OrderType", "order");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
                vnpay.AddRequestData("vnp_TxnRef", vnp_TxnRef);
                vnpay.AddRequestData("vnp_ExpireDate", vietnamTime.AddMinutes(5).ToString("yyyyMMddHHmmss"));

                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                return paymentUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during payment: {ex.Message}");
            }
        }

        private string ReturnedErrorMessageResponseCode(string code)
        {
            switch (code)
            {
                case "00": return "Giao dịch thành công";
                case "07": return "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).";
                case "09": return "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.";
                case "10": return "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần.";
                case "11": return "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.";
                case "12": return "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.";
                case "13": return "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch.";
                case "24": return "Giao dịch không thành công do: Khách hàng hủy giao dịch.";
                case "51": return "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.";
                case "65": return "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.";
                case "75": return "Ngân hàng thanh toán đang bảo trì.";
                case "79": return "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch.";
                case "99": return "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê).";
                default: return "Mã lỗi không hợp lệ";
            }
        }

        private string ReturnedErrorMessageTransactionStatus(string code)
        {
            switch (code)
            {
                case "00": return "Giao dịch thành công";
                case "01": return "Giao dịch chưa hoàn tất";
                case "02": return "Giao dịch bị lỗi";
                case "04": return "Giao dịch đảo (Khách hàng đã bị trừ tiền tại Ngân hàng nhưng GD chưa thành công ở VNPAY)";
                case "05": return "VNPAY đang xử lý giao dịch này (GD hoàn tiền)";
                case "06": return "VNPAY đã gửi yêu cầu hoàn tiền sang Ngân hàng (GD hoàn tiền)";
                case "07": return "Giao dịch bị nghi ngờ gian lận";
                case "09": return "GD Hoàn trả bị từ chối";
                default: return "Mã lỗi không hợp lệ";
            }
        }

        public async Task<ResponsePayment> GetInformationPayment(int userId, VNPayRequestDTO dto)
        {
            string vnp_HashSecret = "UWEORVE5ULXN8YNCLM16TFK1FWPQ0SA9";
            var vnpayData = dto.urlResponse.Split("?")[1];
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (string s in vnpayData.Split("&"))
            {
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    var parts = s.Split("=");
                    if (parts.Length >= 2)
                    {
                        vnpay.AddResponseData(parts[0], parts[1]);
                    }
                }
            }
            string vnpayTranId = vnpay.GetResponseData("vnp_TransactionNo");
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string orderInfo = vnpay.GetResponseData("vnp_OrderInfo")
                                        .Replace("+", " ")
                                        .Replace("%3A", ":");
            string vnp_SecureHash = vnpay.GetResponseData("vnp_SecureHash");
            string terminalID = vnpay.GetResponseData("vnp_TmnCode");
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            string bankCode = vnpay.GetResponseData("vnp_BankCode");
            string transactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            string txnRef = vnpay.GetResponseData("vnp_TxnRef");
            string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string bankTranNo = vnpay.GetResponseData("vnp_BankTranNo");
            string cardType = vnpay.GetResponseData("vnp_CardType");
            string payDate = vnpay.GetResponseData("vnp_PayDate");
            string hashSecret = vnpay.GetResponseData("vnp_HashSecret");

            var responseCodeMessage = ReturnedErrorMessageResponseCode(responseCode);
            var transactionStatusMessage = ReturnedErrorMessageTransactionStatus(transactionStatus);

            VnPayResponse response = new VnPayResponse()
            {
                TransactionId = vnpayTranId,
                OrderInfo = orderInfo,
                Amount = vnp_Amount,
                BankCode = bankCode,
                BankTranNo = bankTranNo,
                CardType = cardType,
                PayDate = payDate,
                ResponseCode = responseCode,
                TransactionStatus = transactionStatus,
                TxnRef = txnRef
            };
            if (vnp_ResponseCode == "00" && transactionStatus == "00")
            {
                await _userService.UpdateUserStatus(userId, 1);
            }
            else
            {
                await _userService.UpdateUserStatus(userId, 2);
            }

            ResponsePayment payment = new ResponsePayment()
            {
                ResponseCodeMessage = responseCodeMessage,
                TransactionStatusMessage = transactionStatusMessage,
                VnPayResponse = response
            };

            return payment;
        }
    }
}
