using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;

namespace CaptoneProject_IOTS_API.Controllers.UserController
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUserServices _userService;
        private readonly ICustomerService customerService;
        private readonly IActivityLogService activityLogService;
        private readonly IUserRequestService userRequestService;
        //================ COMMON =====================
        public CustomerController(
            IUserServices userService,
            IActivityLogService activityLogService,
            ICustomerService customerService,
            IStaffManagerService staffManagerService,
            IUserRequestService userRequestService

        )
        {
            this._userService = userService;
            this.activityLogService = activityLogService;
            this.customerService = customerService;
            this.userRequestService = userRequestService;
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

        [HttpPost("create-customer-user-request-verify-otp")]
        public async Task<IActionResult> CreateRequestVerifyOtpEmail([FromBody] CreateUserRequestDTO request)
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

        [HttpPost("register-customer-user")]
        public async Task<IActionResult> RegisterCustomerAccount([FromBody] UserRegisterDTO payload)
        {
            if (payload.UserInfomation.RoleId != (int)RoleEnum.CUSTOMER)
            {
                return GetActionResult(
                    new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "User role is not customer"
                    }
                );
            }

            return GetActionResult(
                await customerService.RegisterCustomerUser(payload)
            );
        }
    }
}
