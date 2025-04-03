using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.StoreDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_API.Controllers.UserController
{
    [Route("api/trainer")]
    [ApiController]
    public class TrainerController : ControllerBase
    {
        private readonly ITrainerService trainerService;
        private readonly IUserRequestService userRequestService;
        private readonly IActivityLogService activityLogService;
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
        public TrainerController(ITrainerService trainerService,
            IUserRequestService userRequestService,
            IActivityLogService activityLogService)
        {
            this.trainerService = trainerService;
            this.userRequestService = userRequestService;
            this.activityLogService = activityLogService;
        }

        [HttpPost("register-trainer-user")]
        public async Task<IActionResult> RegisterTrainerUser(
            [FromBody] UserRegisterDTO payload
        )
        {
            var response = await trainerService.RegisterTrainerUser(payload);

            return GetActionResult(response);
        }

        [HttpPost("create-trainer-user-request-verify-otp")]
        public async Task<IActionResult> CreateStoreUserRequest([FromBody] CreateUserRequestDTO request)
        {
            var response = await trainerService.CreateTrainerUserRequestVerifyOtp(request.Email);

            return GetActionResult(response);
        }

        [HttpPost("submit-pending-to-approve-trainer-request/{requestId}")]
        public async Task<IActionResult> SubmitPendingToApproveUserRequest(int requestId)
        {
            var res = await userRequestService.UpdateUserRequestStatus(requestId, UserRequestStatusEnum.PENDING_TO_APPROVE);

            return GetActionResult(res);
        }

        [HttpPost("create-or-update-trainer-business-license")]
        public async Task<IActionResult> CreateOrUpdateTrainerBusinessLicenses(
            [FromBody] TrainerBusinessLicensesDTO payload)
        {
            var res = await trainerService.CreateOrUpdateTrainerBusinessLicences(payload);

            return GetActionResult(res);
        }

        [HttpGet("get-trainer-business-license/{trainerId}")]
        [Authorize]
        public async Task<IActionResult> GetBusinessLicenseByTrainerId(int trainerId)
        {
            var res = await trainerService.GetTrainerBusinessLicenseByTrainerId(trainerId);

            return GetActionResult(res);
        }
    }
}
