using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
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
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDTO payload, string returnUrl = null)
        {
            var result = await _orderService.CreateOrder(null, payload, returnUrl);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("check-order-success")]
        public async Task<IActionResult> CheckOrderSuccess([FromBody] VNPayRequestDTO dto)
        {
            var result = await _orderService.CheckOrderSuccessfull(null, dto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("get-order-by-user")]
        public async Task<IActionResult> GetOrderByUser([FromQuery] int? OrderFilterId, [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.GetOrdersByUserPagination(null,payload);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("get-order-by-store-id")]
        public async Task<IActionResult> GetOrderByStoreId([FromQuery] int? OrderFilterId, [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.getOrderByStoreId(null, payload);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("get-order-by-store-id-has-status-order-pending")]
        public async Task<IActionResult> GetOrderByStoreIdStatusOrderIsPending([FromQuery] int? OrderFilterId, [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.getOrderByStoreIdHasStatusPending(null, payload);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("get-all-order-pagination")]
        public async Task<IActionResult> GetAllOrdersPagination([FromQuery] int? OrderFilterId, [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.GetAllOrdersPagination(null, payload);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
