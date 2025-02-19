using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
