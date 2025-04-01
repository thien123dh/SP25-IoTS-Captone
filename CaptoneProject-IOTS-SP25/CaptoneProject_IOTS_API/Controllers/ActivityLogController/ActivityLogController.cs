using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CaptoneProject_IOTS_API.Controllers.ActivityLogController
{
    [Route("api/activity-log")]
    [ApiController]
    [Authorize]
    public class ActivityLogController : ControllerBase
    {
        private readonly IActivityLogService activityLogService;
        //================ COMMON =====================
        public ActivityLogController(IActivityLogService activityLogService)
        {
            this.activityLogService = activityLogService;
        }
        private IActionResult GetActionResult(ResponseDTO response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(response);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("get-pagination-activity-log/{userId}")]
        public async Task<IActionResult> GetPaginationActivityLog(
            int userId,
            [FromBody] PaginationRequest payload
        )
        {
            return GetActionResult(await activityLogService.GetPaginationActivityLog(payload, userId));
        }
    }
}
