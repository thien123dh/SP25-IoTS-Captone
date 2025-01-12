﻿using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;

namespace CaptoneProject_IOTS_API.Controllers.UserRequestController
{
    [Route("api/user-request")]
    [ApiController]
    public class UserRequestController : ControllerBase
    {
        IUserRequestService userRequestService;

        public UserRequestController (
            IUserRequestService userRequestService
        )
        {
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

        [HttpPost("create-email-verify-request")]
        public async Task<IActionResult> CreateEmailVerifyRequest
        (
            [FromBody] CreateUserRequestDTO request
        )
        {
            return GetActionResult(
                await userRequestService.CreateOrUpdateUserRequest(
                    request.Email, 
                    (int) UserRequestStatusEnum.PENDING_TO_VERIFY_OTP
                )
            );
        }

        [HttpPost("listing")]
        public async Task<IActionResult> GetPaginationUserRequest(
            [FromBody] PaginationRequest paginationRequest
        )
        {
            return GetActionResult(
                await userRequestService.GetUserRequestPagination(paginationRequest)
            );
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequestDTO payload)
        {
            return GetActionResult(await userRequestService.VerifyOTP(payload.Email, payload.OTP));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserRequestDetailsById(int id)
        {
            return GetActionResult(await userRequestService.GetUserRequestById(id));
        }
    }
}
