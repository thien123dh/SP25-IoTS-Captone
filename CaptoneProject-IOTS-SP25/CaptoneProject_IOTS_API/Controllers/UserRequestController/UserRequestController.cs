using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;

namespace CaptoneProject_IOTS_API.Controllers.UserRequestController
{
    [Route("api/user-request")]
    [ApiController]
    public class UserRequestController : ControllerBase
    {
        IUserRequestService userRequestService;
        IActivityLogService activityLogService;
        public UserRequestController (
            IUserRequestService userRequestService,
            IActivityLogService activityLogService
        )
        {
            this.userRequestService = userRequestService;
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

        [HttpPost("create-request-verify-email-otp")]
        public async Task<IActionResult> CreateRequestVerifyOtpEmail
        (
            [FromBody] CreateUserRequestDTO request
        )
        {
            return GetActionResult(
                await userRequestService.CreateOrUpdateUserRequest(
                    new UserRequestRequestDTO
                    {
                        Email = request.Email,
                        UserRequestStatus = (int)UserRequestStatusEnum.PENDING_TO_VERIFY_OTP,
                        RoleId = (int)RoleEnum.CUSTOMER
                    }
                )
            );
        }

        [HttpPost("listing")]
        public async Task<IActionResult> GetPaginationUserRequest(
            [FromQuery] int? statusFilter,
            [FromBody] PaginationRequest paginationRequest
        )
        {
            return GetActionResult(
                await userRequestService.GetUserRequestPagination(statusFilter, paginationRequest)
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRequestDetailsById(int id)
        {
            return GetActionResult(await userRequestService.GetUserRequestDetailsById(id));
        }

        [HttpGet("get-user-request-details-by-user-id/{userId}")]
        public async Task<IActionResult> GetUserRequestDetailsByUserId(int userId)
        {
            return GetActionResult(await userRequestService.GetUserRequestDetailsByUserId(userId));
        }


        [HttpPost("approve-user-request/{id}")]
        public async Task<IActionResult> ApproveUserRequest(int id,
            [FromBody] RemarkDTO payload
        )
        {
            var response = await userRequestService.ApproveOrRejectRequestStatus(id, "", isApprove: 1);

            if (response.IsSuccess)
            {
                await activityLogService.CreateUserHistoryTrackingActivityLog("Approved User Request", response?.Data?.userRequestInfo.Email, "Approved");
            }

            return GetActionResult(response);
        }

        [HttpPost("reject-user-request/{id}")]
        public async Task<IActionResult> RejectUserRequest(int id,
            [FromBody] RemarkDTO payload)
        {
            var response = await userRequestService.ApproveOrRejectRequestStatus(id, payload.Remark, isApprove: 0);

            if (response.IsSuccess)
            {
                await activityLogService.CreateUserHistoryTrackingActivityLog("Rejected User Request", response?.Data?.userRequestInfo.Email, "Rejected");
            }

            return GetActionResult(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserRequest(int id)
        {
            var response = await userRequestService.DeleteUserRequestById(id);

            return GetActionResult(response);
        }
    }
}
