using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class CustomerService : ICustomerService
    {
        private readonly IUserRequestService userRequestService;
        private readonly IUserServices _userServices;
        public CustomerService(IUserRequestService userRequestService,
            IUserServices _userServices
        )
        {
            this.userRequestService = userRequestService;
            this._userServices = _userServices;
        }
        public async Task<ResponseDTO> RegisterCustomerUser(UserRegisterDTO payload)
        {
            string otp = payload.Otp;
            CreateUserDTO userInfo = payload.UserInfomation;

            ResponseDTO verifyOtpResponse = await userRequestService.VerifyOTP(userInfo.Email, otp);

            if (!verifyOtpResponse.IsSuccess)
                return verifyOtpResponse;

            GenericResponseDTO<UserResponseDTO> response = await _userServices.CreateOrUpdateUser(0, userInfo, isActive: 1);

            if (!response.IsSuccess)
                return response;

            //Change user request to approve
            userRequestService?.CreateOrUpdateUserRequest(
                new UserRequestRequestDTO {
                    Email = userInfo.Email, 
                    UserRequestStatus = (int)UserRequestStatusEnum.APPROVED, 
                    RoleId = payload.UserInfomation.RoleId
            });

            await _userServices.UpdateUserPassword(response.Data?.Id == null ? 0 : response.Data.Id, payload.Password);

            return response;
        }
    }
}
