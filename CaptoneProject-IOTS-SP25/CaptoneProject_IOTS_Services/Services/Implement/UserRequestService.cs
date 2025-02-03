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
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Http;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class UserRequestService : IUserRequestService
    {
        UserRequestRepository userRequestRepository;
        UserRepository userRepository;
        const int OTP_EXPIRED_MINUTES = 60;
        private readonly IEmailService _emailService;
        private readonly MyHttpAccessor myHttpAccessor;
        private readonly IUserServices _userServices;
        public UserRequestService
        (
            UserRequestRepository userRequestRepository,
            IEmailService emailService,
            UserRepository userRepository,
            MyHttpAccessor myHttpAccessor,
            IUserServices _userServices
        )
        {
            this.userRequestRepository = userRequestRepository;
            _emailService = emailService;
            this.userRepository = userRepository;
            this.myHttpAccessor = myHttpAccessor;
            this._userServices = _userServices;
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

            //return otp.Trim();
            return "123456";
        }

        public async Task<GenericResponseDTO<UserRequestResponseDTO>> CreateOrUpdateUserRequest(UserRequestRequestDTO payload)
        {
            UserRequest? userRequest = await userRequestRepository.GetByEmail(payload.Email);

            userRequest = (userRequest == null) ? new UserRequest() : userRequest;

            userRequest.ActionBy = myHttpAccessor.GetLoginUserId();

            userRequest.Status = payload.UserRequestStatus;

            userRequest.Email = payload.Email;

            userRequest.RoleId = payload.RoleId;

            if (payload.UserRequestStatus == (int) UserRequestConstant.UserRequestStatusEnum.PENDING_TO_VERIFY_OTP)
            {
                string otp = GenerateOTP();

                userRequest.OtpCode = otp.Trim();

                userRequest.ExpiredDate = DateTime.Now.AddMinutes(OTP_EXPIRED_MINUTES);
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

            UserRequest response = await userRequestRepository.GetByEmail(payload.Email);

            //TODO - HARDCODE
            var link = "https://www.facebook.com/thien.nguyen.1257604";

            var emailTemplate = EmailTemplateConst.CreateStaffOrManagerEmailTemplate(userRequest.Email, userRequest.OtpCode, link);

            _emailService.SendEmailAsync(userRequest.Email, emailTemplate.Subject, emailTemplate.Body);

            return new GenericResponseDTO<UserRequestResponseDTO>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = UserRequestMapper.MappingToUserRequestResponseDTO(response)
            };
        }

        public async Task<ResponseDTO> GetUserRequestPagination(int? userRequestStatusFilter, 
            PaginationRequest paginationRequest)
        {
            PaginationResponseDTO<UserRequest> paginationData = userRequestRepository.GetPaginate(
                filter: ur => (
                    ur.Email.Contains(paginationRequest.SearchKeyword)
                    &&
                    ur.RoleId != (int)RoleEnum.CUSTOMER
                    &&
                    (
                        (
                            ur.RoleId == (int)RoleEnum.STAFF
                        ) || (
                            ur.Status != (int)UserRequestStatusEnum.PENDING_TO_VERIFY_OTP
                            &&
                            ur.Status != (int)UserRequestStatusEnum.VERIFIED_OTP
                        )
                    )
                    &&
                    (userRequestStatusFilter == null || userRequestStatusFilter == ur.Status)
                ),
                orderBy: ur => ur.OrderByDescending(ur => ur.CreatedDate),
                includeProperties: "StatusNavigation,Role",
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

            if (userRequest == null)
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "User Request Email cannot be found"
                };

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

        public async Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> GetUserRequestDetailsById(int requestId)
        {
            UserRequest userRequest = await userRequestRepository.GetById(requestId);

            if (userRequest == null)
                return new GenericResponseDTO<UserRequestDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.USER_REQUEST_NOT_FOUND,
                    StatusCode = HttpStatusCode.NotFound
                };

            var userResponse = (await _userServices.GetUserDetailsByEmail(userRequest.Email));

            if (!userResponse.IsSuccess)
                return new GenericResponseDTO<UserRequestDetailsResponseDTO>
                {
                    IsSuccess = false,
                    StatusCode = userResponse.StatusCode,
                    Message = userResponse.Message
                };

            UserDetailsResponseDTO? userInfo = userResponse.Data;

            return new GenericResponseDTO<UserRequestDetailsResponseDTO>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = HttpStatusCode.OK,
                Data = UserRequestMapper.MappingToUserRequestDetailsResponseDTO(userRequest, userInfo)
            };  
        }

        public async Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> GetUserRequestDetailsByUserId(int userId)
        {
            User user = userRepository.GetById(userId);

            var userRequest = await userRequestRepository.GetByEmail(user.Email);

            return await GetUserRequestDetailsById(userRequest.Id);
        }

        public async Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> ApproveOrRejectRequestStatus(int requestId, string? remark, int isApprove)
        {
            var userRequest = await userRequestRepository.GetById(requestId);

            if (userRequest == null)
                return new GenericResponseDTO<UserRequestDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.USER_REQUEST_NOT_FOUND,
                    StatusCode = HttpStatusCode.NotFound
                };

            if (userRequest.Status != (int)UserRequestStatusEnum.PENDING_TO_APPROVE)
                return new GenericResponseDTO<UserRequestDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = "User Request Status must be Pending to Approve",
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (isApprove <= 0 && (remark == null || remark.Trim() == ""))
            {
                return new GenericResponseDTO<UserRequestDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Please enter the reason why you rejected",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                userRequest.Status = isApprove > 0 ? (int)UserRequestStatusEnum.APPROVED : (int)UserRequestStatusEnum.REJECTED;
                userRequest.Remark = remark;
                userRequest = userRequestRepository.Update(userRequest);
            }
            catch (Exception ex)
            {
                return new GenericResponseDTO<UserRequestDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            if (isApprove > 0)
            {
                User user = await userRepository.GetUserByEmail(userRequest.Email);

                await _userServices.UpdateUserStatus(user.Id, 1);
            }

            return await GetUserRequestDetailsById(userRequest.Id);
        }

        public async Task<GenericResponseDTO<UserRequest>> DeleteUserRequestById(int id)
        {
            var userRequest = await userRequestRepository.GetById(id);

            if (userRequest == null)
                return new GenericResponseDTO<UserRequest>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.USER_REQUEST_NOT_FOUND,
                    StatusCode = HttpStatusCode.NotFound
                };

            userRequestRepository.Remove(userRequest);

            return new GenericResponseDTO<UserRequest>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = HttpStatusCode.OK,
                Data = userRequest
            };
        }
    }
}
