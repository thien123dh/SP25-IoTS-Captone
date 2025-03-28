using Azure;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
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
    [Authorize]
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

        [HttpPost("customer/get-pagination")]
        public async Task<IActionResult> GetOrderByUser(
            [FromQuery] OrderItemStatusEnum? orderItemStatusFilter,
            [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.GetOrdersByUserPagination(payload, orderItemStatusFilter);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId, 
            [FromQuery] OrderItemStatusEnum? orderItemStatusFilter)
        {
            var response = await _orderService.GetOrdersDetailsByOrderId(orderId, orderItemStatusFilter);

            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("store-trainer/get-pagination")]
        public async Task<IActionResult> GetOrderByStoreId(
            [FromQuery] OrderItemStatusEnum? orderItemStatusFilter,
            [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.GetOrderByStoreOrTrainerPagination(payload, orderItemStatusFilter);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("order-status/packing/{orderId}")]
        public async Task<IActionResult> UpdateOrderDetailToPackingByStoreId(int orderId)
        {
            var result = await _orderService.UpdateOrderDetailToPackingByStoreId(orderId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("order-status/delivering/{orderId}")]
        public async Task<IActionResult> UpdateOrderDetailToDeliveringByStoreId(int orderId)
        {
            var result = await _orderService.UpdateOrderDetailToDeliveringByStoreId(orderId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("order-status/pending-to-feedback/{orderId}")]
        public async Task<IActionResult> UpdateOrderDetailToDeliveredByStoreId(int orderId)
        {
            var result = await _orderService.UpdateOrderDetailToPendingToFeedbackByStoreId(orderId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("admin-manager/get-pagination")]
        public async Task<IActionResult> GetAllOrdersPagination(
            [FromQuery] OrderItemStatusEnum? orderItemStatusFilter,
            [FromQuery] int? OrderFilterId, [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.GetAdminOrdersPagination(payload, orderItemStatusFilter);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
