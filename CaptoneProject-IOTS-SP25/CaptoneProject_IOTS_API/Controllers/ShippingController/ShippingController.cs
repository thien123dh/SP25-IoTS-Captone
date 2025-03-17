using CaptoneProject_IOTS_BOs.DTO.GHTKDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CaptoneProject_IOTS_API.Controllers.ShippingController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IGHTKService _shippingService;

        public ShippingController(IGHTKService shippingService)
        {
            _shippingService = shippingService;
        }

        [HttpPost("create/{orderId}")]
        public async Task<IActionResult> CreateShipment(int orderId)
        {

            var success = await _shippingService.CreateShipmentAsync(orderId);
            if (success)
            {
                return Ok(new { message = "Đơn hàng đã được gửi tới GHTK thành công." });
            }
            return BadRequest(new { message = "Không thể gửi đơn hàng tới GHTK." });
        }


        [HttpPost("get-fee")]
        public async Task<IActionResult> GetFee([FromBody] ShippingFeeRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            var responseContent = await _shippingService.GetShippingFeeAsync(request);

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return BadRequest("Không lấy được phí vận chuyển.");
            }

            return Ok(responseContent);
        }
    }
}
