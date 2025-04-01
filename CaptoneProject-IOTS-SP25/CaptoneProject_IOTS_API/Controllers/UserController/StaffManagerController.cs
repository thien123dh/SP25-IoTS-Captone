using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;

namespace CaptoneProject_IOTS_API.Controllers.UserController
{
    [Route("api/staff-manager")]
    [ApiController]
    public class StaffManagerController : ControllerBase
    {
        private readonly IUserServices _userService;
        private readonly ICustomerService customerService;
        private readonly IStaffManagerService staffManagerService;
        private readonly IActivityLogService activityLogService;
        //================ COMMON =====================
        public StaffManagerController(
            IUserServices userService,
            IActivityLogService activityLogService,
            ICustomerService customerService,
            IStaffManagerService staffManagerService
        )
        {
            this._userService = userService;
            this.activityLogService = activityLogService;
            this.customerService = customerService;
            this.staffManagerService = staffManagerService;
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
        [HttpPost("create-staff-manager-request")]
        public async Task<IActionResult> CreateStaffRequest([FromBody] CreateUserDTO payload)
        {
            var res = await staffManagerService.CreateStaffOrManager(payload);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Create a staff/manager account with email {payload.Email}");
            }

            return GetActionResult(res);
        }

        [HttpPost("verify-staff-manager-otp")]
        public async Task<IActionResult> VerifyOtp(
            [FromBody] StaffManagerVerifyOtpRequest payload
        )
        {
            return GetActionResult(
                await staffManagerService.StaffManagerVerifyOTP(
                    payload.OTP,
                    payload.RequestId,
                    (int)UserRequestStatusEnum.APPROVED,
                    payload.password
            )
            );
        }
    }
}
