using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ReportDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.FeedbackController
{
    [Route("api/report")]
    [ApiController]
    [Authorize]
    public class ReportController : MyBaseController.MyBaseController
    {
        private readonly IReportService ratingService;
        private readonly IActivityLogService activityLogService;
        public ReportController(IReportService ratingService, IActivityLogService activityLog)
        {
            this.ratingService = ratingService;
            this.activityLogService = activityLog;
        }

        [HttpPost]
        [Route("get-pagination")]
        public async Task<IActionResult> GetReportPagination(
            [FromBody] PaginationRequest payload,
            [FromQuery] ReportStatusEnum? statusFilter)
        {
            var res = await ratingService.GetReportPagination((int?)statusFilter, payload).ConfigureAwait(false);

            return GetActionResult(res);
        }

        [HttpPost]
        [Route("approve/{reportId}")]
        public async Task<IActionResult> HandledSuccessReport(int reportId)
        {
            var res = await ratingService.HandledSuccessReportAsync(reportId, true);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Approved Report ID {reportId}");
            }

            return GetActionResult(res);
        }

        [HttpPost]
        [Route("refund/{reportId}")]
        public async Task<IActionResult> RefundedReport(int reportId, 
            [FromBody] DtoRefundReportRequest payload)
        {
            var res = await ratingService.RefundReportAsync(reportId, payload);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Rejected Report ID {reportId}");
            }

            return GetActionResult(res);
        }
    }
}
