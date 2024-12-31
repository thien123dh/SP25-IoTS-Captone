using Azure;
using Azure.Core;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CaptoneProject_IOTS_API.Controllers.UserController
{
    [Route("api/user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userService;

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

        [HttpGet("listing")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsers();

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
    }
}
