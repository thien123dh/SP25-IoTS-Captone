using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class UserRequestService : IUserRequestService
    {
        UserRequestRepository userRequestRepository;
        const int OTP_EXPIRED_MINUTES = 2;
        public UserRequestService
        (
            UserRequestRepository userRequestRepository
        )
        {
            this.userRequestRepository = userRequestRepository;
        }

        private string GenerateOTP()
        {
            var random = new Random();
            var otp = "";
            int otpLength = 6;

            for (int i = 0; i < otpLength; i++)
            {
                otp += random.Next(0, 10);
            }

            return otp;
        }

        public async Task<ResponseDTO> CreateOrUpdateUserRequest(
            string email, 
            int userRequestStatus
        )
        {
            UserRequest? userRequest = await userRequestRepository.GetByEmail(email);

            userRequest = (userRequest == null) ? new UserRequest() : userRequest;

            userRequest.Status = userRequestStatus;

            Console.WriteLine("UserStatus: " + userRequestStatus);

            userRequest.Email = email;

            Console.WriteLine("Enum: " + UserRequestConstant.UserRequestStatusEnum.PENDING_TO_VERIFY_OTP);

            if (userRequestStatus == (int) UserRequestConstant.UserRequestStatusEnum.PENDING_TO_VERIFY_OTP)
            {
                
                string otp = GenerateOTP();

                Console.WriteLine("OTP: " + otp);

                userRequest.OtpCode = otp;

                userRequest.ExpiredDate = DateTime.Now.AddMinutes(OTP_EXPIRED_MINUTES);

                //userRequest.RoleId = role;
            }

            Console.WriteLine("No access if");

            if (userRequest.Id > 0)
            {
                userRequest.ActionDate = DateTime.Now;

                userRequestRepository.Update(userRequest);
                
            }
            else
            {
                userRequestRepository.Create(userRequest);
            }
            MapService<UserRequest, UserRequestResponseDTO> mapper = new MapService<UserRequest, UserRequestResponseDTO>();
            
            UserRequest response = await userRequestRepository.GetByEmail(email);

            //Console.WriteLine("Response: " + response);
            
            return new ResponseDTO
            {
                IsSuccess = true,
                Data = mapper.MappingTo(response)
            };
        }

        public async Task<ResponseDTO> GetUserRequestPagination(PaginationRequest paginationRequest)
        {
            PaginationResponse<UserRequest> response = userRequestRepository.GetPaginate(
                filter: null,
                orderBy: null,
                includeProperties: "",
                pageIndex: paginationRequest.PageIndex,
                pageSize: paginationRequest.PageSize
            );

            return new ResponseDTO
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = response
            };
        }
    }
}
