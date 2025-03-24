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
            [FromQuery] OrderItemStatusEnum? orderItemStatus,
            [FromQuery] int? OrderFilterId, 
            [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.GetOrdersByUserPagination((int?)orderItemStatus, null, payload);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var response = await _orderService.GetOrdersDetailsByOrderId(orderId);

            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("store/get-pagination")]
        public async Task<IActionResult> GetOrderByStoreId(
            [FromQuery] OrderItemStatusEnum? orderItemStatus,
            [FromQuery] int? OrderFilterId, 
            [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.GetOrderByStorePagination((int?)orderItemStatus, null, payload);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("change-status-order-detail-to-packing-{orderId})")]
        public async Task<IActionResult> updateOrderDetailToPackingByStoreId(int orderId)
        {
            var result = await _orderService.updateOrderDetailToPackingByStoreId(orderId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("change-status-order-detail-to-delevering-{orderId})")]
        public async Task<IActionResult> updateOrderDetailToDeliveringByStoreId(int orderId)
        {
            var result = await _orderService.updateOrderDetailToDeleveringByStoreId(orderId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("change-status-order-detail-to-delevered-{orderId})")]
        public async Task<IActionResult> updateOrderDetailToDeliveredByStoreId(int orderId, int storeId)
        {
            var result = await _orderService.updateOrderDetailToDeleveredByStoreId(orderId, storeId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("get-order-by-store-id-has-status-order-pending")]
        public async Task<IActionResult> GetOrderByStoreIdStatusOrderIsPending([FromQuery] int? OrderFilterId, [FromBody] PaginationRequest payload)
        {
            var result = await _orderService.GetOrderByStoreIdHasStatusPending(null, payload);

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
