using Azure;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.NotificationDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
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
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;
using static System.Net.Mime.MediaTypeNames;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class UserRequestService : IUserRequestService
    {
        UserRequestRepository userRequestRepository;
        UserRepository userRepository;
        const int OTP_EXPIRED_MINUTES = 60;
        private readonly IEmailService _emailService;
        private readonly IUserServices _userServices;
        private readonly IEnvironmentService environmentService;
        private readonly INotificationService notificationService;
        public UserRequestService
        (
            UserRequestRepository userRequestRepository,
            IEmailService emailService,
            UserRepository userRepository,
            IUserServices _userServices,
            IEnvironmentService environmentService,
            INotificationService notificationService
        )
        {
            this.userRequestRepository = userRequestRepository;
            this._emailService = emailService;
            this.userRepository = userRepository;
            this._userServices = _userServices;
            this.environmentService = environmentService;
            this.notificationService = notificationService;
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
            int? loginUserId = _userServices.GetLoginUserId();
            
            UserRequest? userRequest = await userRequestRepository.GetByEmail(payload.Email);

            userRequest = (userRequest == null) ? new UserRequest() : userRequest;
            
            userRequest.ActionBy = loginUserId;
            userRequest.Status = payload.UserRequestStatus;
            userRequest.Email = payload.Email;
            userRequest.RoleId = payload.RoleId;

            if (payload.UserRequestStatus == (int) UserRequestConstant.UserRequestStatusEnum.PENDING_TO_VERIFY_OTP)
            {
                string otp = GenerateOTP();

                userRequest.OtpCode = otp.Trim();

                userRequest.ExpiredDate = DateTime.Now.AddMinutes(OTP_EXPIRED_MINUTES);
            }

            if (userRequest.Id > 0) //Update
            {
                userRequest.ActionDate = DateTime.Now;

                userRequestRepository.Update(userRequest);
            }
            else
            {
                userRequest.CreatedDate = DateTime.Now;
                userRequest.CreatedBy = loginUserId;

                userRequestRepository.Create(userRequest);
            }

            try
            {
                UserRequest response = await userRequestRepository.GetByEmail(payload.Email);

                string url = environmentService.GetFrontendDomain();

                var link = url + "/" + response.Id;
                
                try
                {
                    var emailTemplate = _emailService.GetStaffManagerOtpEmailTemplate(userRequest.OtpCode, link, userRequest.Email);

                    _emailService.SendEmailAsync(userRequest.Email, emailTemplate.Subject, emailTemplate.Body);
                } catch (Exception ex)
                {

                }

                return ResponseService<UserRequestResponseDTO>.OK(UserRequestMapper.MappingToUserRequestResponseDTO(response));
            
            } catch (Exception ex)
            {
                return ResponseService<UserRequestResponseDTO>.BadRequest(ex.Message);
            }
            
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
                            ur.RoleId == (int)RoleEnum.STAFF || ur.RoleId == (int)RoleEnum.MANAGER
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

            return ResponseService<Object>
                .OK(PaginationMapper<UserRequest, UserRequestResponseDTO>
                    .MapTo(UserRequestMapper.MappingToUserRequestResponseDTO, paginationData));
            
        }

        public async Task<ResponseDTO> VerifyOTP(string email, string otp)
        {
            UserRequest userRequest = await userRequestRepository.GetByEmail(email);

            if (userRequest == null)
                return ResponseService<Object>.NotFound("User Request Email cannot be found");

            if (userRequest.Status != (int)UserRequestConstant.UserRequestStatusEnum.PENDING_TO_VERIFY_OTP)
            {
                return ResponseService<Object>.BadRequest(ExceptionMessage.EMAIL_ALREADY_VERIFIED);
            }

            if (userRequest == null)
            {
                return ResponseService<Object>.BadRequest("The verified email request cannot be found");
            }

            if (otp.Trim().CompareTo(userRequest.OtpCode.Trim()) == 0)
            {
                //if (DateTime.Now > userRequest.ExpiredDate)
                //{
                //    return ResponseService<Object>.BadRequest(ExceptionMessage.EXPIRED_OTP);
                //} else
                //{
                //    return ResponseService<Object>.OK(null);
                //}
                return ResponseService<Object>.OK(null);
            }

            return ResponseService<Object>.BadRequest(ExceptionMessage.INCORRECT_OTP);
        }

        public async Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> GetUserRequestDetailsById(int requestId)
        {
            UserRequest userRequest = await userRequestRepository.GetById(requestId);

            if (userRequest == null)
                return ResponseService<UserRequestDetailsResponseDTO>.NotFound(ExceptionMessage.USER_REQUEST_NOT_FOUND);
                
            var userResponse = (await _userServices.GetUserDetailsByEmail(userRequest.Email));

            if (!userResponse.IsSuccess)
                return ResponseService<UserRequestDetailsResponseDTO>.CastTypeErrorResponse(userResponse);

            UserDetailsResponseDTO? userInfo = userResponse.Data;

            return ResponseService<UserRequestDetailsResponseDTO>
                .OK(UserRequestMapper.MappingToUserRequestDetailsResponseDTO(userRequest, userInfo));
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
                return ResponseService<UserRequestDetailsResponseDTO>
                    .NotFound(ExceptionMessage.USER_REQUEST_NOT_FOUND);
                

            if (userRequest.Status != (int)UserRequestStatusEnum.PENDING_TO_APPROVE)
                return ResponseService<UserRequestDetailsResponseDTO>
                    .BadRequest("You cannot Reject or Approve this Request");

            if (isApprove <= 0 && (remark == null || remark.Trim() == ""))
            {
                return ResponseService<UserRequestDetailsResponseDTO>
                    .BadRequest("Please enter the reason why you rejected");
            }

            try
            {
                userRequest.Status = isApprove > 0 ? (int)UserRequestStatusEnum.APPROVED : (int)UserRequestStatusEnum.REJECTED;
                userRequest.Remark = remark;
                userRequest = userRequestRepository.Update(userRequest);

                var noti = new NotificationRequestDTO();

                if (isApprove > 0)
                {
                    noti = new NotificationRequestDTO {
                        Title = "Your user request has been approve. Welcome to Iot Trading Website",
                        Content = "Your user request has been approve. Welcome to Iot Trading Website",
                        EntityId = userRequest.Id,
                        EntityType = (int)EntityTypeEnum.USER_REQUEST
                    };
                }
                else
                {
                    noti = new NotificationRequestDTO
                    {
                        Title = "Your user request has been rejected. Please check the reason",
                        Content = "Your user request has been rejected. Please check the reason",
                        EntityId = userRequest.Id,
                        EntityType = (int)EntityTypeEnum.USER_REQUEST
                    };
                }

                var save = new List<NotificationRequestDTO>()
                    {
                        noti
                    };

                _ = notificationService.CreateUserNotification(save);
            }
            catch (Exception ex)
            {
                return ResponseService<UserRequestDetailsResponseDTO>
                    .BadRequest(ex.Message);
            }

            //if (isApprove > 0)
            //{
            //    User user = await userRepository.GetUserByEmail(userRequest.Email);

            //    await _userServices.UpdateUserStatus(user.Id, (int)UserStatusEnum.ACTIVE);
            //}

            return await GetUserRequestDetailsById(userRequest.Id);
        }

        public async Task<GenericResponseDTO<UserRequest>> DeleteUserRequestById(int id)
        {
            var userRequest = await userRequestRepository.GetById(id);

            if (userRequest == null)
                return ResponseService<UserRequest>.NotFound(ExceptionMessage.USER_REQUEST_NOT_FOUND);           

            try
            {
                userRequestRepository.Remove(userRequest);

                return ResponseService<UserRequest>.OK(userRequest);

            } catch (Exception ex)
            {
                return ResponseService<UserRequest>.BadRequest(ex.Message);
            }
        }

        public async Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> UpdateUserRequestStatus(int requestId, UserRequestStatusEnum status)
        {
            var updateItem = await userRequestRepository.GetById(requestId);

            if (updateItem == null)
                return ResponseService<UserRequestDetailsResponseDTO>
                    .NotFound(ExceptionMessage.USER_REQUEST_NOT_FOUND);

            updateItem.Status = (int)status;

            try
            {
                updateItem = userRequestRepository.Update(updateItem);

                return await GetUserRequestDetailsById(updateItem.Id);

            } catch (Exception ex)
            {
                return ResponseService<UserRequestDetailsResponseDTO>
                    .NotFound(ex.Message);
            }
        }
    }
}
