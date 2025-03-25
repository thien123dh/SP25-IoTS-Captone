using Azure;
using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;

namespace CaptoneProject_IOTS_API.Controllers.VNPayController
{
    [Route("api/vnpay/")]
    [ApiController]
    [Authorize]
    public class VNPayController : ControllerBase
    {
        private IVNPayService  _vnPayService;
        public VNPayController(IVNPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpGet("vnpay-create-pay-with-account")]
        public async Task<IActionResult> PayWithUserId([FromQuery] long amount, string returnUrl = null)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _vnPayService.CallAPIPayByUserId(userId, returnUrl, amount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("check-payment")]
        public async Task<IActionResult> Check([FromBody] VNPayRequestDTO dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _vnPayService.GetInformationPayment(userId, dto);
            return Ok(result);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> RefundPayment(long amount, string TxnRef, string transactionDate)
        {
            var vnp_Api = "http://sandbox.vnpayment.vn/merchant_webapi/merchant.html";
            var vnp_HashSecret = "PJLU0FHO";
            var vnp_TmnCode = "4RY7BQN7ED5YFS7YR4TS3YONAJPGYYFL";
            var vnpay = new VnPayLibrary();
            var strDatax = "";
            if (string.IsNullOrEmpty(TxnRef))
            {
                return BadRequest(new { message = "Mã giao dịch không được để trống!" });
            }

            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_RequestId = DateTime.Now.Ticks.ToString();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION.ToString());
            vnpay.AddRequestData("vnp_Command", "refund");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode.ToString());
            vnpay.AddRequestData("vnp_TransactionType", "03");
            vnpay.AddRequestData("vnp_CreateBy", "Hệ thống Student Volunteer");
            vnpay.AddRequestData("vnp_TxnRef", TxnRef.ToString());
            vnpay.AddRequestData("vnp_Amount", amount.ToString() + "00");
            vnpay.AddRequestData("vnp_OrderInfo", "Hoàn tiền từ chiến dịch: " + TxnRef.ToString());
            vnpay.AddRequestData("vnp_TransDate", transactionDate);
            vnpay.AddRequestData("vnp_CreateDate", vnp_CreateDate);
            vnpay.AddRequestData("vnp_IpAddr", "1");

            var refundtUrl = vnpay.CreateRequestUrl(vnp_Api, vnp_HashSecret);

            var request = (HttpWebRequest)WebRequest.Create(refundtUrl);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
                if (stream != null)
                    using (var reader = new StreamReader(stream))
                    {
                        strDatax = reader.ReadToEnd();
                    }
            if (strDatax.Contains("ResponseCode=00"))
            {

                
            }
            else
            {
                throw new Exception("Lỗi trong quá trình hoàn tiền");
            }
            return Ok("Đã hoàn tiền thành công chiến dịch: " + TxnRef.ToString());
        }
    }
}

