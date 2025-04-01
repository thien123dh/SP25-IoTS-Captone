using CaptoneProject_IOTS_BOs.DTO.FeedbackDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.FeedbackController
{
    [Route("api/feedback")]
    [ApiController]
    public class FeedbackController : MyBaseController.MyBaseController
    {
        private readonly IFeedbackService feedbackService;
        private readonly IActivityLogService activityLogService;
        public FeedbackController(IFeedbackService feedbackService, IActivityLogService activityLogService)
        {
            this.feedbackService = feedbackService;
            this.activityLogService = activityLogService;
        }

        [HttpPost("product/get-pagination")]
        public async Task<IActionResult> GetProductFeedbackPagination(
            [FromBody] GenericPaginationRequest<ProductRequestDTO> payload    
        )
        {
            var res = await feedbackService.GetFeedbackPagination(payload.AdvancedFilter, payload.PaginationRequest);

            return GetActionResult(res);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateOrderFeedback(
            [FromBody] StoreOrderFeedbackRequestDTO payload    
        )
        {
            var res = await feedbackService.CreateOrderFeedback(payload);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Create new feedback with order ID {payload.OrderId} and seller ID {payload.SellerId}");
            }

            return GetActionResult(res);
        }
    }
}
