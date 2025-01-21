using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class StaffManagerService : IStaffManagerService
    {
        private readonly UserRepository _userRepository;
        private readonly UserRoleRepository _userRoleRepository;
        private readonly ITokenServices _tokenGenerator;
        private readonly PasswordHasher<string> _passwordHasher;
        private readonly IUserRequestService userRequestService;
        private readonly UserRequestRepository userRequestRepository;
        private readonly MyHttpAccessor httpAccessor;
        private readonly IUserServices _userServices;
        public StaffManagerService(
            UserRepository userService,
            ITokenServices tokenGenerator,
            UserRoleRepository userRoleRepository,
            IUserRequestService userRequestService,
            UserRequestRepository userRequestRepository,
            MyHttpAccessor httpAccessor,
            IUserServices _userServices
        )
        {
            _userRepository = userService;
            _userRoleRepository = userRoleRepository;
            _tokenGenerator = tokenGenerator;
            _passwordHasher = new PasswordHasher<string>();
            this.userRequestService = userRequestService;
            this.userRequestRepository = userRequestRepository;
            this.httpAccessor = httpAccessor;
            this._userServices = _userServices;
        }
        public async Task<ResponseDTO> CreateStaffOrManager(CreateUserDTO payload)
        {
            if (payload.RoleId != (int)RoleEnum.MANAGER && payload.RoleId != (int)RoleEnum.STAFF)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = ExceptionMessage.INVALID_STAFF_MANAGER_ROLE
                };
            }

            GenericResponseDTO<UserResponseDTO> createUserResponse = await _userServices.CreateUser(0, payload, isActive: 0);

            if (!createUserResponse.IsSuccess)
                return createUserResponse;

            GenericResponseDTO<UserRequestResponseDTO> createRequestResponse = await userRequestService.
                CreateOrUpdateUserRequest(
                new UserRequestRequestDTO
                {
                    Email = payload.Email,
                    UserRequestStatus = (int)UserRequestStatusEnum.APPROVED,
                    RoleId = payload.RoleId
                });

            return new ResponseDTO
            {
                IsSuccess = createRequestResponse.IsSuccess,
                StatusCode = createRequestResponse.StatusCode,
                Message = createRequestResponse.Message,
                Data = new
                {
                    requestId = createRequestResponse?.Data?.Id
                }
            };
        }

        public async Task<ResponseDTO> StaffManagerVerifyOTP(
                    string otp,
                    int requestId,
                    int requestStatusId,
                    string password
        )
        {
            UserRequest userRequest = await userRequestRepository.GetById(requestId);

            string email = userRequest.Email;

            if (userRequest == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.USER_REQUEST_NOT_FOUND,
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            User user = await _userRepository.GetUserByEmail(userRequest.Email);

            ResponseDTO otpResponse = await userRequestService.VerifyOTP(email, otp);

            if (otpResponse.IsSuccess)
            {
                if (user != null)
                {
                    user.Password = _passwordHasher.HashPassword(null, password);
                    user.UpdatedDate = DateTime.Now;
                    user.IsActive = 1;
                    //Update user status to active
                    _userRepository.Update(user);
                }

                userRequest.Status = requestStatusId;
                UserRequest response = userRequestRepository.Update(userRequest);

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = HttpStatusCode.OK,
                    Data = new
                    {
                        RequestId = response.Id
                    }
                };
            }

            return otpResponse;
        }
    }
}
