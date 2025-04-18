﻿using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.FeedbackDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

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
        public async Task<IActionResult> ApproveReport(int reportId)
        {
            var res = await ratingService.ApproveOrRejectReport(reportId, true);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Approved Report ID {reportId}");
            }

            return GetActionResult(res);
        }

        [HttpPost]
        [Route("reject/{reportId}")]
        public async Task<IActionResult> RejectReport(int reportId)
        {
            var res = await ratingService.ApproveOrRejectReport(reportId, false);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Rejected Report ID {reportId}");
            }

            return GetActionResult(res);
        }
    }
}
