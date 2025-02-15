using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.StoreDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
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
    public class TrainerService : ITrainerService
    {
        private readonly IUserRequestService _userRequestService;
        private readonly UserRequestRepository _userRequestRepository;
        private readonly IUserServices _userService;
        private readonly UserRepository _userRepository;
        private readonly TrainerBusinessLicensesRepository trainerBusinessLicensesRepository;
        public TrainerService(
            IUserRequestService _userRequestService,
            UserRequestRepository _userRequestRepository,
            IUserServices _userService,
            UserRepository _userRepository,
            BusinessLicenseRepository businessLicenseRepository,
            TrainerBusinessLicensesRepository trainerBusinessLicensesRepository)
        {
            this._userRequestService = _userRequestService;
            this._userService = _userService;
            this._userRequestRepository = _userRequestRepository;
            this._userRepository = _userRepository;
            this.trainerBusinessLicensesRepository = trainerBusinessLicensesRepository;
        }

        public async Task<GenericResponseDTO<TrainerBusinessLicense>> CreateOrUpdateTrainerBusinessLicences(TrainerBusinessLicensesDTO payload)
        {
            var loginUserId = _userService.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<TrainerBusinessLicense>.Unauthorize("Please login to update license");

            int trainerId = (int)loginUserId;

            var trainer = _userRepository.GetById(trainerId);

            if (trainer == null)
                return ResponseService<TrainerBusinessLicense>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);

            var businessLicense = trainerBusinessLicensesRepository.GetByTrainerId(trainerId);

            businessLicense = businessLicense == null ? new TrainerBusinessLicense() : businessLicense;

            businessLicense.BackIdentification = payload.BackIdentification;
            businessLicense.FrontIdentification = payload.FrontIdentification;
            businessLicense.BusinessLicences = payload.BusinessLicences;
            //businessLicense.LiscenseNumber = payload.LiscenseNumber;
            businessLicense.TrainerId = trainer.Id;
            businessLicense.IssueDate = payload.IssueDate;
            businessLicense.ExpiredDate = payload.ExpiredDate;
            businessLicense.IssueBy = payload.IssueBy;

            try
            {
                if (businessLicense.Id > 0) //Update
                    businessLicense = trainerBusinessLicensesRepository.Update(businessLicense);
                else //Create
                    businessLicense = trainerBusinessLicensesRepository.Create(businessLicense);

                return
                    ResponseService<TrainerBusinessLicense>.OK(businessLicense);
            }
            catch (Exception ex)
            {
                return ResponseService<TrainerBusinessLicense>.BadRequest(ex.Message);
            }
        }

        public async Task<GenericResponseDTO<UserRequestResponseDTO>> CreateTrainerUserRequestVerifyOtp(string email)
        {
            User user = await _userRepository.GetUserByEmail(email);

            if (user != null)
                return ResponseService<UserRequestResponseDTO>.NotFound(ExceptionMessage.USER_EXIST_EXCEPTION);

            return await _userRequestService.CreateOrUpdateUserRequest(
                new UserRequestRequestDTO
                {
                    Email = email,
                    UserRequestStatus = (int)UserRequestStatusEnum.PENDING_TO_VERIFY_OTP,
                    RoleId = (int)RoleEnum.TRAINER
                });
        }

        public async Task<GenericResponseDTO<TrainerBusinessLicense>> GetTrainerBusinessLicenseByTrainerId(int trainerId)
        {
            var loginUserId = _userService.GetLoginUserId();

            var isAdminOrManager = await _userService.CheckLoginUserRole(RoleEnum.ADMIN) || await _userService.CheckLoginUserRole(RoleEnum.STAFF);

            if (!isAdminOrManager
                &&
                loginUserId != trainerId)
            {
                return ResponseService<TrainerBusinessLicense>.BadRequest("You don't have permission to access");
            }

            var trainer = _userRepository.GetById(trainerId);

            if (trainer == null)
                return ResponseService<TrainerBusinessLicense>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);

            var res = trainerBusinessLicensesRepository.GetByTrainerId(trainerId);

            if (res == null)
                return ResponseService<TrainerBusinessLicense>.NotFound("Trainer Business License Cannot be Found");

            return ResponseService<TrainerBusinessLicense>.OK(res);
        }

        public async Task<GenericResponseDTO<UserResponseDTO>> RegisterTrainerUser(UserRegisterDTO payload)
        {
            string otp = payload.Otp;
            CreateUserDTO userInfo = payload.UserInfomation;
            int? loginUserId = _userService.GetLoginUserId();

            if (payload.UserInfomation.RoleId != (int)RoleEnum.TRAINER)
                return ResponseService<UserResponseDTO>.BadRequest(ExceptionMessage.INVALID_TRAINER_ROLE);

            if (userInfo.Email == "" || userInfo.Email == null)
            {
                return ResponseService<UserResponseDTO>.BadRequest(ExceptionMessage.USER_EMAIL_INVALID);
            }

            ResponseDTO verifyOtpResponse = await _userRequestService.VerifyOTP(userInfo.Email, otp);

            if (!verifyOtpResponse.IsSuccess)
                return new GenericResponseDTO<UserResponseDTO>
                {
                    IsSuccess = false,
                    StatusCode = verifyOtpResponse.StatusCode,
                    Message = verifyOtpResponse.Message
                };

            //User status change to 2 ==> PENDING
            GenericResponseDTO<UserResponseDTO> response = await _userService.CreateOrUpdateUser(0, userInfo, isActive: 2);

            if (!response.IsSuccess)
                return response;

            //Change user request to Verified_OTP
            _userRequestService?.CreateOrUpdateUserRequest(
                new UserRequestRequestDTO
                {
                    Email = userInfo.Email,
                    UserRequestStatus = (int)UserRequestStatusEnum.VERIFIED_OTP,
                    RoleId = (int)RoleEnum.TRAINER
                });

            await _userService.UpdateUserPassword(response.Data?.Id == null ? 0 : response.Data.Id, payload.Password);

            try
            {

                var license = new TrainerBusinessLicense
                {
                    TrainerId = response.Data.Id
                };

                //Auto create default lisences
                trainerBusinessLicensesRepository.Create(license);
            }
            catch (Exception e)
            {
                return ResponseService<UserResponseDTO>.BadRequest(e.Message);
            }

            return response;
        }
    }
}
