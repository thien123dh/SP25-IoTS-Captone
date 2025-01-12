using Azure;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.Services.Interface;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class UserRequestService : IUserRequestService
    {
        UserRequestRepository userRequestRepository;
        UserRepository userRepository;
        const int OTP_EXPIRED_MINUTES = 60;
        private readonly IEmailService _emailService;
        public UserRequestService
        (
            UserRequestRepository userRequestRepository,
            IEmailService emailService,
            UserRepository userRepository
        )
        {
            this.userRequestRepository = userRequestRepository;
            _emailService = emailService;
            this.userRepository = userRepository;
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

            //return otp;
            //return otp.Trim();
            return "123456";
        }

        public async Task<GenericResponseDTO<UserRequestResponseDTO>> CreateOrUpdateUserRequest(
            string email, 
            int userRequestStatus
/*            string decision, string reason*/
        )
        {
            UserRequest? userRequest = await userRequestRepository.GetByEmail(email);

            userRequest = (userRequest == null) ? new UserRequest() : userRequest;

            userRequest.ActionBy = userRepository.GetLoginUser()?.Id;

            userRequest.Status = userRequestStatus;

            userRequest.Email = email;

            if (userRequestStatus == (int) UserRequestConstant.UserRequestStatusEnum.PENDING_TO_VERIFY_OTP)
            {
                string otp = GenerateOTP();

                userRequest.OtpCode = otp.Trim();

                userRequest.ExpiredDate = DateTime.Now.AddMinutes(OTP_EXPIRED_MINUTES);

                //userRequest.RoleId = role;
            }

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

            var subject = "Verify Account";  // Set the email's subject to the decision
            var body = $"Dear {userRequest.Email},\n\n" +
                       $"Test.\n\n" +
                       $"OTP: {userRequest.OtpCode}\n\n" +
                       $"Best regards,\nThe Admin Team";

            _emailService.SendEmailAsync(userRequest.Email, subject, body);

            return new GenericResponseDTO<UserRequestResponseDTO>
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

        public async Task<ResponseDTO> VerifyOTP(string email, string otp)
        {
            UserRequest userRequest = await userRequestRepository.GetByEmail(email);

            if (userRequest == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Request doesn't exist"
                };
            }

            if (otp.Trim().CompareTo(userRequest.OtpCode.Trim()) == 0)
            {
                if (DateTime.Now > userRequest.ExpiredDate)
                {
                    return new ResponseDTO
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "The OTP is expired"
                    };
                } else
                {
                    return new ResponseDTO
                    {
                        IsSuccess = true,
                        StatusCode = HttpStatusCode.OK,
                        Message = "Ok"
                    };
                }
            }

            return new ResponseDTO
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "OTP is not correct"
            };
        }

        public async Task<GenericResponseDTO<UserRequestDetailsResponse>> GetUserRequestById(int requestId)
        {
            UserRequest userRequest = userRequestRepository.GetById(requestId);

            if (userRequest == null)
            {
                return new GenericResponseDTO<UserRequestDetailsResponse>
                {
                    IsSuccess = true,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = ExceptionMessage.USER_REQUEST_NOT_FOUND
                };
            }

            User user = await userRepository.GetUserByEmail(userRequest.Email);

            UserRequestDetailsResponse response = new UserRequestDetailsResponse
            {
                Id = userRequest.Id,
                Email = userRequest.Email,
                StatusNavigation = userRequest.StatusNavigation,
                userDetails = UserMapper.mapToUserDetailResponse(user)
            };

            return new GenericResponseDTO<UserRequestDetailsResponse>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = response
            };
        }
    }
}
