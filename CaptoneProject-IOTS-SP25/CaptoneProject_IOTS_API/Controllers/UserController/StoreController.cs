using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData;
using System.Net;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_API.Controllers.UserController
{
    [Route("api/store")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly IUserRequestService _userRequestService;
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

        public StoreController(IStoreService _storeService,
            IUserRequestService userRequestService)
        {
            this._storeService = _storeService;
            this._userRequestService = userRequestService;
        }

        [HttpPost("register-store-user")]
        public async Task<IActionResult> RegisterStoreUser(
            [FromBody] UserRegisterDTO payload 
        )
        {
            var response = await _storeService.RegisterStoreUser(payload);

            return GetActionResult(response);
        }

        [HttpPost("create-store-user-request-verify-otp")]
        public async Task<IActionResult> CreateStoreUserRequest([FromBody] CreateUserRequestDTO request)
        {
            var response = await _storeService.CreateStoreUserRequestVerifyOtp(request.Email);

            return GetActionResult(response);
        }

        [HttpPost("submit-store/{userId}")]
        public async Task<IActionResult> CreateStoreByUserId(int userId,
            [FromBody] StoreRequestDTO payload)
        {
            var response = await _storeService.SubmitStoreInfomation(userId, payload);

            return GetActionResult(response);
        }

        [HttpGet("get-store-details-by-user-id/{userId}")]
        public async Task<IActionResult> GetStoreDetailsByUserId(int userId)
        {
            var response = await _storeService.GetStoreDetailsByUserId(userId);

            return GetActionResult(response);
        }
        [HttpPost("get-pagination-stores")]
        public async Task<IActionResult> GetPaginationStores([FromBody] PaginationRequest paginationRequest)
        {
            var response = await _storeService.GetPaginationStores(paginationRequest);

            return GetActionResult(response);
        }
    }
}
