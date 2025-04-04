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
        public ReportController(IReportService ratingService)
        {
            this.ratingService = ratingService;
        }

        [HttpPost]
        [Route("get-pagination")]
        public async Task<IActionResult> GetReportPagination(
            [FromBody] PaginationRequest payload
        )
        {
            var res = await ratingService.GetReportPagination(payload);

            return GetActionResult(res);
        }

        [HttpPost]
        [Route("approve/{reportId}")]
        public async Task<IActionResult> ApproveReport(int reportId)
        {
            var res = await ratingService.ApproveOrRejectReport(reportId, true);

            return GetActionResult(res);
        }

        [HttpPost]
        [Route("reject/{reportId}")]
        public async Task<IActionResult> RejectReport(int reportId)
        {
            var res = await ratingService.ApproveOrRejectReport(reportId, false);

            return GetActionResult(res);
        }
    }
}
