using Azure;
using Azure.Core;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;
using System.Security.Claims;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CaptoneProject_IOTS_API.Controllers.UserController
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userService;
        private readonly IActivityLogService activityLogService;
        //================ COMMON =====================
        public UsersController(
            IUserServices userService,
            IActivityLogService activityLogService
        )
        {
            this._userService = userService;
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

        [HttpPost("listing")]

/*        [Authorize(Roles = "Admin")]*/
        public async Task<IActionResult> GetUserPagination(
            [FromBody] PaginationRequest paginationRequest,
            [FromQuery] int? role
        )
        {
            var response = await _userService.GetUsersPagination(paginationRequest, roleId: role);

            return GetActionResult(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            var response = await _userService.GetUserDetailsById(id);

            return GetActionResult(response);
        }

        [HttpPut("update-user-role/{id}")]
        public async Task<IActionResult> UpdateUserRole(
            int id,
            [FromBody] UpdateUserRoleRequestDTO payload)
        {
            var response = await _userService.UpdateUserRole(id, payload.RoleIdList);

            return GetActionResult(response);
        }

        [HttpPut("activate-user/{id}")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var response = await _userService.UpdateUserStatus(id, isActive: 1);

            if (response.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Activated User ID {id}");
            }

            return GetActionResult(response);
        }

        [HttpPut("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdateLoginUserPassword([FromBody] ChangePasswordRequestDTO payload)
        {
            var res = await _userService.UserChangePassword(payload);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Updated user password");
            }

            return GetActionResult(res);
        }

        [HttpPut("deactive-user/{id}")]
        public async Task<IActionResult> DeactiveUser(int id)
        {
            var response = await _userService.UpdateUserStatus(id, isActive: 0);

            if (response.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Deactivated User ID {id}");
            }

            return GetActionResult(response);
        }
        [HttpGet("get-user-by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var response = await _userService.GetUserDetailsByEmail(email);

            return GetActionResult(response);
        }

        [Authorize]
        [HttpGet("get-user-login-info")]
        public async Task<IActionResult> GetUserLoginInfo()
        {
            var response = await _userService.GetUserLoginInfo();
            return Ok(response);
        }

        [HttpPut("update-user-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile(
            [FromBody] UpdateUserDTO payload
        )
        {
            var response = await _userService.UpdateUserProfile(payload);

            if (response.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Updated user profile");
            }

            return Ok(response);
        }

        [HttpPut("update-user-avatar")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAvatar(
            [FromBody] UpdateUserAvatarDTO payload
        )
        {
            var response = await _userService.UpdateUserAvatar(payload);

            if (response.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Updated user avatar");
            }
            return Ok(response);
        }
    }
}
