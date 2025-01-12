﻿using Azure;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CaptoneProject_IOTS_API.Controllers.UserController
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userService;
        //================ COMMON =====================
        public UsersController(IUserServices userService)
        {
            this._userService = userService;
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
        public async Task<IActionResult> UpdateUserRole (
            int id,
            [FromBody] UpdateUserRoleRequestDTO payload)
        {
            var response = await _userService.UpdateUserRole(id, payload.RoleIdList);

            return GetActionResult(response);
        }
        [HttpPut("activate-user/{id}")]
        public async Task<IActionResult> ActivateUser( int id)
        {
            var response = await _userService.UpdateUserStatus(id, isActive: 1);

            return GetActionResult(response);
        }
        [HttpPut("deactive-user/{id}")]
        public async Task<IActionResult> DeactiveUser(int id)
        {
            var response = await _userService.UpdateUserStatus(id, isActive: 0);

            return GetActionResult(response);
        }
        [HttpGet("get-user-by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var response = await _userService.GetUserDetailsByEmail(email);

            return GetActionResult(response);
        }
        //================ COMMON =====================

        //================ ADMIN ======================
        [HttpPost("create-staff-manager-request")]
        public async Task<IActionResult> CreateStaffRequest([FromBody] CreateUserDTO payload)
        {
            return GetActionResult(

                await _userService.CreateStaffOrManager(payload)

            );
        }

        //================ ADMIN ======================
        //================ STAFF/MANAGER ======================
        [HttpPost("verify-staff-manager-otp")]
        public async Task<IActionResult> VerifyOtp (
            [FromBody] StaffManagerVerifyOtpRequest payload
        )
        {
            return GetActionResult(
                await _userService.StaffManagerVerifyOTP (
                    payload.OTP, 
                    payload.RequestId, 
                    (int)UserRequestStatusEnum.APPROVED, 
                    payload.password
            )
            );
        }
        //================ STAFF/MANAGER ======================

        //================ CUSTOMER ===========================
        //[HttpPost("create-verify-customer-user")]
        //public async Task<IActionResult> 


        //================ CUSTOMER ===========================
        [HttpPost("register-verify-otp-customer")]
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
                await _userService.RegisterUser(payload)
            );
        }
        
        //
        //================ Decode lay role =================/
        [Authorize]
        [HttpGet("get-user-login-info")]
        public async Task<IActionResult> GetUserLoginInfo()
        {
            var response = await _userService.GetUserLoginInfo();
            return Ok(response);
        }


    }
}
