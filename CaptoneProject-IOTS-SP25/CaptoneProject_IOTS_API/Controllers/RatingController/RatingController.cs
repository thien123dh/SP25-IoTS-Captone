using CaptoneProject_IOTS_BOs.DTO.FeedbackDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_API.Controllers.FeedbackController
{
    [Route("api/rating")]
    [ApiController]
    public class RatingController : MyBaseController.MyBaseController
    {
        private readonly IRatingService ratingService;
        public RatingController(IRatingService ratingService)
        {
            this.ratingService = ratingService;
        }

        [HttpPost]
        [Authorize]
        [Route("report/get-pagination")]
        public async Task<IActionResult> GetReportPagination(
            [FromBody] PaginationRequest payload
        )
        {
            var res = await ratingService.GetReportPagination(payload);

            return GetActionResult(res);
        }
    }
}
