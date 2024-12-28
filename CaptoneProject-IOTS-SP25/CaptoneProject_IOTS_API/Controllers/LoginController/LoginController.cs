using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CaptoneProject_IOTS_API.Controllers.LoginController
{
    [ApiController]
    [Route("api/")]
    public class LoginController : ControllerBase
    {
        private readonly IUserServices _userService;

        public LoginController(IUserServices userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var response = await _userService.LoginUserAsync(request.Email, request.Password);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(response);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
