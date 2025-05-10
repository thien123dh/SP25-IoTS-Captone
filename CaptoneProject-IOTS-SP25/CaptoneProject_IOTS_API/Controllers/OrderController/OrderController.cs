using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.RefundDTO;
using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CaptoneProject_IOTS_API.Controllers.OrderController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : MyBaseController.MyBaseController
    {
        private readonly IOrderService _orderService;
        private readonly IActivityLogService activityLogService;
        public OrderController(IOrderService orderService, IActivityLogService activityLogService)
        {
            _orderService = orderService;
            this.activityLogService = activityLogService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequestDTO payload, string returnUrl = null)
        {
            var result = await _orderService.CreateOrder(null, payload, returnUrl);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("create-order-by-mobile")]
        public async Task<IActionResult> CreateOrderByMobile([FromBody] OrderRequestDTO payload)
        {
            var result = await _orderService.CreateOrderByMobile(null, payload);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("check-order-success")]
        public async Task<IActionResult> CheckOrderSuccess([FromBody] VNPayRequestDTO dto)
        {
            var result = await _orderService.CheckOrderSuccessfull(null, dto);

            if (result.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Created new order");
            }

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("check-order-success-by-mobile")]
        public async Task<IActionResult> CheckOrderSuccessByMobile([FromBody] VNPayRequestDTO dto)
        {
            var result = await _orderService.CheckOrderSuccessfullByMobile(null, dto);

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

        [HttpPost("create-cash-payment-order")]
        public async Task<IActionResult> CreateCashPaymentOrder([FromBody] OrderRequestDTO payload)
        {
            var result = await _orderService.CreateCashPaymentOrder(payload);

            if (result.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Created new cash payment order");
            }

            return GetActionResult(result);
        }

        [HttpPost("order-status/packing/{orderId}")]
        public async Task<IActionResult> UpdateOrderDetailToPackingByStoreId(int orderId,
            [FromBody] CreateOrderWarrantyInfo payload)
        {
            var result = await _orderService.UpdateOrderDetailToPacking(orderId, payload);

            if (result.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Updated order to packing with ID {orderId}");
            }

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("order-status/delivering/{orderId}")]
        public async Task<IActionResult> UpdateOrderDetailToDeliveringByStoreId(int orderId)
        {
            var result = await _orderService.UpdateOrderDetailToDelivering(orderId);

            if (result.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Updated order to delivering with ID {orderId}");
            }

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("order-status/pending-to-feedback/{orderId}")]
        public async Task<IActionResult> UpdateOrderDetailToDeliveredByStoreId(int orderId,
            [FromQuery][Required] int sellerId)
        {
            var result = await _orderService.UpdateOrderDetailToPendingToFeedback(orderId, sellerId);

            if (result.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Updated order to delivered with ID {orderId} and seller ID {sellerId}");
            }

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

        [HttpPost("order-status/success-order/{orderId}")]
        public async Task<IActionResult> UpdateOrderItemToSuccess(int orderId)
        {
            var res = await _orderService.UpdateOrderDetailToSuccess(orderId);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Updated order to success with ID {orderId}");
            }

            return GetActionResult(res);
        }

        [HttpPost("order-status/cancelled/{orderId}")]
        public async Task<IActionResult> UpdateOrderItemToCancelled(int orderId,
            [FromBody] CreateRefundRequestDTO payload)
        {
            var res = await _orderService.UpdateOnlinePaymentOrderDetailToCancel(orderId, payload);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Cancelled order with ID {orderId}");
            }

            return GetActionResult(res);
        }

        [HttpPost("order-status/cash-payment/cancelled/{orderId}")]
        public async Task<IActionResult> UpdateOrderItemToCancelled(int orderId)
        {
            var res = await _orderService.UpdateCashpaymentOrderDetailToCancel(orderId);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Cancelled order with ID {orderId}");
            }

            return GetActionResult(res);
        }
    }
}
