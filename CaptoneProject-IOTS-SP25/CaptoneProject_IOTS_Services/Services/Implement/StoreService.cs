using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.StoreDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class StoreService : IStoreService
    {
        private readonly MyHttpAccessor _myHttpAccessor;
        private readonly IUserRequestService _userRequestService;
        private readonly UserRequestRepository _userRequestRepository;
        private readonly IUserServices _userService;
        private readonly UserRepository _userRepository;
        private readonly StoreRepository _storeRepository;
        private readonly StoreAttachmentRepository _storeAttachmentRepository;
        private readonly BusinessLicenseRepository businessLicenseRepository;
        public StoreService (MyHttpAccessor _myHttpAccessor,
            IUserRequestService _userRequestService,
            UserRequestRepository _userRequestRepository,
            IUserServices _userService,
            UserRepository _userRepository,
            StoreRepository storeRepository,
            StoreAttachmentRepository storeAttachmentRepository,
            BusinessLicenseRepository businessLicenseRepository)
        {
            this._myHttpAccessor = _myHttpAccessor;
            this._userRequestService = _userRequestService;
            this._userService = _userService;
            this._userRequestRepository = _userRequestRepository;
            this._userRepository = _userRepository;
            this._storeRepository = storeRepository;
            this._storeAttachmentRepository = storeAttachmentRepository;
            this.businessLicenseRepository = businessLicenseRepository;
        }

        private async Task<GenericResponseDTO<StoreDetailsResponseDTO>> GetDetailsStoreById(int storeId)
        {
            Store store = _storeRepository.GetById(storeId);

            if (store == null)
                return new GenericResponseDTO<StoreDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.STORE_NOTFOUND,
                    StatusCode = HttpStatusCode.NotFound
                };

            return new GenericResponseDTO<StoreDetailsResponseDTO>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = HttpStatusCode.OK,
                Data = StoreMapper.MapToStoreDetailsResponseDTO(store)
            };
        }

        private async Task<ResponseDTO> CreateOrUpdateStoreAttachments(int storeId, List<StoreAttachmentRequestDTO>? payload)
        {
            Store store = _storeRepository.GetById(storeId);

            List<StoreAttachment> dbList = store?.StoreAttachmentsNavigation?.ToList();

            int? loginUserId = _myHttpAccessor.GetLoginUserId();

            var removeList = dbList?.Where(sa => (payload?.SingleOrDefault(item => sa.Id == item.Id) == null)).ToList();

            var newList = payload?.Where(item => dbList?.SingleOrDefault(s => s.Id == item.Id) == null).Select(item => new StoreAttachment
            {
                ImageUrl = item.ImageUrl,
                StoreId = storeId,
                CreatedDate = DateTime.Now,
                createdBy = loginUserId == null ? 0 : loginUserId
            });

            if (removeList != null)
                await _storeAttachmentRepository.RemoveAsync(removeList);

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = HttpStatusCode.OK,
                Data = (newList == null) ? null : await _storeAttachmentRepository.CreateAsync(newList)
            };
        }

        public async Task<GenericResponseDTO<StoreDetailsResponseDTO>> SubmitStoreInfomation(int userId, StoreRequestDTO payload)
        {
            var response = await CreateOrUpdateStoreByUserId(userId, payload);

            if (!response.IsSuccess)
                return response;

            var user = _userRepository.GetById(userId);

            var userRequestResponse = await _userRequestService.CreateOrUpdateUserRequest(
                new UserRequestRequestDTO
                {
                    Email = user.Email,
                    UserRequestStatus = (int)UserRequestStatusEnum.PENDING_TO_APPROVE,
                    RoleId = (int)RoleEnum.STORE
                });

            if (!userRequestResponse.IsSuccess)
                return new GenericResponseDTO<StoreDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = userRequestResponse.Message,
                    StatusCode = userRequestResponse.StatusCode
                };

            return await GetStoreDetailsByUserId(userId);
        }

        public async Task<GenericResponseDTO<StoreDetailsResponseDTO>> CreateOrUpdateStoreByUserId(int userId, StoreRequestDTO payload)
        {
            User user = await _userRepository.GetUserById(userId);

            if (!(user?.UserRoles?.Count(ur => ur.RoleId == (int)RoleEnum.STORE) > 0))
                return new GenericResponseDTO<StoreDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.INVALID_STORE_ROLE,
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (user == null)
                return new GenericResponseDTO<StoreDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.USER_DOESNT_EXIST,
                    StatusCode = HttpStatusCode.NotFound
                };

            Store store = _storeRepository.GetByUserId(userId);

            store = store == null ? new Store
            {
                IsActive = 2 //Default is pending
            } : store;

            int? loginUserId = _myHttpAccessor.GetLoginUserId();

            //Set data 
            store.Name = payload.Name;
            store.Description = payload.Description;
            store.OwnerId = userId;
            store.UpdatedBy = loginUserId;
            store.UpdatedDate = DateTime.Now;
            store.ImageUrl = payload.ImageUrl;
            store.ContactNumber = payload.ContactNumber;
            store.Summary = payload.Summary;
            store.Address = payload.Address;
            //

            if (store.Id > 0) //Update
                store = _storeRepository.Update(store);
            else
            {
                store.CreatedDate = DateTime.Now;
                store.CreatedBy = loginUserId;

                store = _storeRepository.Create(store);
            }

            var response = await CreateOrUpdateStoreAttachments(store.Id, payload.StoreAttachments);

            return await GetDetailsStoreById(store.Id);
        }

        public async Task<GenericResponseDTO<UserRequestResponseDTO>> CreateStoreUserRequestVerifyOtp(string email)
        {
            User user = await _userRepository.GetUserByEmail(email);

            if (user != null)
                return new GenericResponseDTO<UserRequestResponseDTO>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.USER_EXIST_EXCEPTION,
                    StatusCode = HttpStatusCode.BadRequest
                };

            return await _userRequestService.CreateOrUpdateUserRequest(
                new UserRequestRequestDTO
                {
                    Email = email,
                    UserRequestStatus = (int)UserRequestStatusEnum.PENDING_TO_VERIFY_OTP,
                    RoleId = (int)RoleEnum.STORE
                });
        }

        public async Task<GenericResponseDTO<UserResponseDTO>> RegisterStoreUser(UserRegisterDTO payload)
        {
            string otp = payload.Otp;
            CreateUserDTO userInfo = payload.UserInfomation;
            int? loginUserId = _userService.GetLoginUserId();

            if (payload.UserInfomation.RoleId != (int)RoleEnum.STORE)
                return new GenericResponseDTO<UserResponseDTO>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.INVALID_STORE_ROLE,
                    StatusCode = HttpStatusCode.BadRequest
                };

            if (userInfo.Email == "" || userInfo.Email == null)
            {
                return new GenericResponseDTO<UserResponseDTO>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = ExceptionMessage.USER_EMAIL_INVALID
                };
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
                    RoleId = (int)RoleEnum.STORE
                });

            await _userService.UpdateUserPassword(response.Data?.Id == null ? 0 : response.Data.Id, payload.Password);
            
            try
            {
                //Auto create store information
                var store = new Store
                {
                    OwnerId = response.Data.Id,
                    CreatedBy = loginUserId,
                    UpdatedBy = loginUserId,
                    IsActive = 0
                };

                store = _storeRepository.Create(store);

                var license = new BusinessLicenses
                {
                    storeId = store.Id
                };

                //Auto create default lisences
                businessLicenseRepository.Create(license);

            } catch (Exception e)
            {
                return ResponseService<UserResponseDTO>.BadRequest(e.Message);
            }

            return response;
        }

        public async Task<GenericResponseDTO<StoreDetailsResponseDTO>> GetStoreDetailsByUserId(int userId)
        {
            Store store = _storeRepository.GetByUserId(userId);

            if (store == null)
                return new GenericResponseDTO<StoreDetailsResponseDTO> {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Message = ExceptionMessage.STORE_NOTFOUND
                };

            return await GetDetailsStoreById(store.Id);
        }

        public async Task<ResponseDTO> GetPaginationStores(PaginationRequest paginationRequest)
        {
            PaginationResponseDTO<Store> res = _storeRepository.GetPaginate(
                filter: s => (
                    s.Name.Contains(paginationRequest.SearchKeyword)
                    &&
                    s.Owner.IsActive == 1
                ),
                orderBy: null,
                includeProperties: "Owner",
                pageIndex: paginationRequest.PageIndex,
                pageSize: paginationRequest.PageSize
            );

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = HttpStatusCode.OK,
                Data = PaginationMapper<Store, StoreResponseDTO>.MappingTo(StoreMapper.MapToStoreResponse, res)
            };
        }

        public async Task<GenericResponseDTO<BusinessLicenses>> CreateOrUpdateBusinessLicences(BusinessLicensesDTO payload)
        {
            int storeId = payload.StoreId;

            var store = _storeRepository.GetById(storeId);

            if (store == null)
                return new GenericResponseDTO<BusinessLicenses>
                {
                    IsSuccess = false,
                    Message = "Not Found",
                    StatusCode = HttpStatusCode.NotFound
                };

            var businessLicense = businessLicenseRepository.GetByStoreId(storeId);

            businessLicense = businessLicense == null ? new BusinessLicenses() : businessLicense;

            //var saveItem = BusinessLicensesMapper.MapToBusinessLicenses(payload);
            //saveItem.Id = businessLicense == null ? saveItem.Id : businessLicense.Id;

            businessLicense.BackIdentification = payload.BackIdentification;
            businessLicense.FrontIdentification = payload.FrontIdentification;
            businessLicense.BusinessLicences = payload.BusinessLicences;
            businessLicense.LiscenseNumber = payload.LiscenseNumber;
            businessLicense.storeId = payload.StoreId;
            businessLicense.IssueDate = payload.IssueDate;
            businessLicense.ExpiredDate = payload.ExpiredDate;
            businessLicense.IssueBy = payload.IssueBy;

            try
            {
                if (businessLicense.Id > 0) //Update
                    businessLicense = businessLicenseRepository.Update(businessLicense);
                else //Create
                    businessLicense = businessLicenseRepository.Create(businessLicense);

                return new GenericResponseDTO<BusinessLicenses>
                {
                    IsSuccess = true,
                    Message = "Success",
                    StatusCode = HttpStatusCode.OK,
                    Data = businessLicense
                };
            } catch (Exception ex)
            {
                return new GenericResponseDTO<BusinessLicenses>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
        }

        public async Task<GenericResponseDTO<BusinessLicenses>> GetBusinessLicencesByStoreId(int storeId)
        {
            var store = _storeRepository.GetById(storeId);

            if (store == null)
                return ResponseService<BusinessLicenses>.NotFound("Store cannot be found");

            var res = businessLicenseRepository.GetByStoreId(storeId);

            if (res == null)
                return ResponseService<BusinessLicenses>.NotFound("License cannot be found");

            return ResponseService<BusinessLicenses>.OK(res);
        }

        public async Task<GenericResponseDTO<StoreDetailsResponseDTO>> GetStoreDetailsByStoreId(int storeId)
        {
            var store = _storeRepository.GetById(storeId);

            if (store == null)
                return ResponseService<StoreDetailsResponseDTO>.NotFound("Store doesn't exist");

            return await  GetStoreDetailsByUserId(store.OwnerId);
        }
    }
}
