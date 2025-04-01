using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.CashoutRequestDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace CaptoneProject_IOTS_API.Controllers.CashoutRequestController
{
    [Route("api/cashout-request")]
    [ApiController]
    [Authorize]
    public class CashoutRequestController : MyBaseController.MyBaseController
    {
        private readonly ICashoutService cashoutService;
        private readonly IActivityLogService activityLogService;
        public CashoutRequestController(ICashoutService cashoutService, IActivityLogService activityLogService)
        {
            this.cashoutService = cashoutService;
            this.activityLogService = activityLogService;
        }

        [HttpPost("get-pagination")]
        public async Task<IActionResult> GetPagination(
            [FromQuery] CashoutRequestStatusEnum? statusFilter,
            [FromBody] PaginationRequest payload)
        {
            var res = await cashoutService.GetPaginationCashoutRequest((int?)statusFilter, payload);

            return GetActionResult(res);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCashoutRequest(
            [FromBody] CreateCashoutRequestDTO payload)
        {
            var res = await cashoutService.CreateCashoutRequest(payload);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Create new request to cashout with ID {res?.Data?.Id}");
            }

            return GetActionResult(res);
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> CreateCashoutRequest(int id)
        {
            var res = await cashoutService.ApproveOrRejectCashoutRequest(id, true);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Approved cashout request with ID {id}");
            }

            return GetActionResult(res);
        }


        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectCashoutRequest(
            int id, 
            [FromBody] RemarkDTO remarks)
        {
            var res = await cashoutService.ApproveOrRejectCashoutRequest(id, false, remarks);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Rejected cashout request with ID {id}");
            }

            return GetActionResult(res);
        }
    }
}
