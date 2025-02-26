using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.OrderController
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDTO payload)
        {
            var result = await _orderService.CreateOrder(null, payload);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("check-order-success")]
        public async Task<IActionResult> CheckOrderSuccess([FromBody] OrderRequestDTO payload)
        {
            var result = await _orderService.CheckOrderSuccessfull(null, payload);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
