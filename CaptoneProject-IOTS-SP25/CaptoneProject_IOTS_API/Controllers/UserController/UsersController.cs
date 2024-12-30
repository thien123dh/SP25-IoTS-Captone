using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CaptoneProject_IOTS_API.Controllers.UserController
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userService;

        public UsersController(IUserServices userService)
        {
            this._userService = userService;
        }
        [HttpGet]
        public async Task<ResponseDTO> GetAllUsers()
        {
            return await _userService.GetAllUsers();
        }
        [HttpPut("activate-user")]
        public async Task<ResponseDTO> ActivateUser(int userId)
        {
            return await _userService.UpdateUserStatus(userId, isActive: 1);
        }
        [HttpPut("deactive-user")]
        public async Task<ResponseDTO> DeactiveUser(int userId)
        {
            return await _userService.UpdateUserStatus(userId, isActive: 0);
        }

    }
}
