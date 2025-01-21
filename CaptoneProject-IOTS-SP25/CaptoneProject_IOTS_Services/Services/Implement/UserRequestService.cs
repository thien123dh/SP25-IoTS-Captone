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
using Microsoft.AspNetCore.Http;
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
        private readonly MyHttpAccessor myHttpAccessor;
        public UserRequestService
        (
            UserRequestRepository userRequestRepository,
            IEmailService emailService,
            UserRepository userRepository,
            MyHttpAccessor myHttpAccessor
        )
        {
            this.userRequestRepository = userRequestRepository;
            _emailService = emailService;
            this.userRepository = userRepository;
            this.myHttpAccessor = myHttpAccessor;
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
        )
        {
            UserRequest? userRequest = await userRequestRepository.GetByEmail(email);

            userRequest = (userRequest == null) ? new UserRequest() : userRequest;

            userRequest.ActionBy = myHttpAccessor.GetLoginUserId();

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
                userRequest.CreatedDate = DateTime.Now;
                userRequestRepository.Create(userRequest);
            }

            UserRequest response = await userRequestRepository.GetByEmail(email);

            var emailTemplate = EmailTemplateConst.CreateStaffOrManagerEmailTemplate(userRequest.Email, userRequest.OtpCode);

            _emailService.SendEmailAsync(userRequest.Email, emailTemplate.Subject, emailTemplate.Body);

            return new GenericResponseDTO<UserRequestResponseDTO>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = (await GetUserRequestById(response.Id)).Data
            };
        }

        public async Task<ResponseDTO> GetUserRequestPagination(int? userRequestStatusFilter, PaginationRequest paginationRequest)
        {
            PaginationResponseDTO<UserRequest> paginationData = userRequestRepository.GetPaginate(
                filter: ur => (
                    ur.Email.Contains(paginationRequest.SearchKeyword)
                    &&
                    (userRequestStatusFilter == null || userRequestStatusFilter == ur.Status)
                ),
                orderBy: null,
                includeProperties: "StatusNavigation",
                pageIndex: paginationRequest.PageIndex,
                pageSize: paginationRequest.PageSize
            );

            return new ResponseDTO
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = PaginationMapper<UserRequest, UserRequestResponseDTO>
                    .mappingTo(UserRequestMapper.MappingToUserRequestResponseDTO, paginationData)
            };
        }

        public async Task<ResponseDTO> VerifyOTP(string email, string otp)
        {
            UserRequest userRequest = await userRequestRepository.GetByEmail(email);

            if (userRequest.Status != (int)UserRequestConstant.UserRequestStatusEnum.PENDING_TO_VERIFY_OTP)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = ExceptionMessage.EMAIL_ALREADY_VERIFIED
                };
            }

            if (userRequest == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "The verified email request cannot be found"
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
                        Message = ExceptionMessage.EXPIRED_OTP
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
                Message = ExceptionMessage.INCORRECT_OTP
            };
        }

        public async Task<GenericResponseDTO<UserRequestResponseDTO>> GetUserRequestById(int requestId)
        {
            UserRequest userRequest = await userRequestRepository.GetById(requestId);

            if (userRequest == null)
            {
                return new GenericResponseDTO<UserRequestResponseDTO>
                {
                    IsSuccess = true,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = ExceptionMessage.USER_REQUEST_NOT_FOUND
                };
            }

            User user = await userRepository.GetUserByEmail(userRequest.Email);

            var response = UserRequestMapper.MappingToUserRequestResponseDTO(userRequest);

            return new GenericResponseDTO<UserRequestResponseDTO>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = response
            };
        }
    }
}
