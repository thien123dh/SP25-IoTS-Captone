using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.AddressDTO;
using CaptoneProject_IOTS_BOs.DTO.GHTKDTO;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
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
        private readonly IUserServices _userService;
        private readonly UserRepository _userRepository;
        private readonly StoreRepository _storeRepository;
        private readonly StoreAttachmentRepository _storeAttachmentRepository;
        private readonly BusinessLicenseRepository businessLicenseRepository;
        private readonly IGHTKService _ghtkService;
        private readonly UnitOfWork unitOfWork;

        public StoreService (MyHttpAccessor _myHttpAccessor,
            IUserRequestService _userRequestService,
            IUserServices _userService,
            UserRepository _userRepository,
            StoreRepository storeRepository,
            StoreAttachmentRepository storeAttachmentRepository,
            BusinessLicenseRepository businessLicenseRepository,
            UnitOfWork unitOfWork,
            IGHTKService ghtkService)
        {
            this._myHttpAccessor = _myHttpAccessor;
            this._userRequestService = _userRequestService;
            this._userService = _userService;
            this._userRepository = _userRepository;
            this._storeRepository = storeRepository;
            this._storeAttachmentRepository = storeAttachmentRepository;
            this.businessLicenseRepository = businessLicenseRepository;
            this._ghtkService = ghtkService;
            this.unitOfWork = unitOfWork;
        }
        public int CountStoreNumberOfProducts(int storeOwnerId)
        {
            var numberOfProduct = unitOfWork.IotsDeviceRepository.Search(item => item.CreatedBy == storeOwnerId && item.IsActive > 0)
                .Count();

            numberOfProduct += unitOfWork.ComboRepository.Search(item => item.CreatedBy == storeOwnerId && item.IsActive > 0)
                .Count();

            return numberOfProduct;
        }

        public async Task<StoreDetailsResponseDTO> BuildToStoreDetailsResponseDTO(Store store)
        {
            var res = StoreMapper.MapToStoreDetailsResponseDTO(store);

            res.StoreNumberOfProducts = CountStoreNumberOfProducts(store.OwnerId);

            return res;
        }

        private async Task<GenericResponseDTO<StoreDetailsResponseDTO>> GetDetailsStoreById(int storeId)
        {
            var store = _storeRepository.GetById(storeId);

            if (store == null)
                return new GenericResponseDTO<StoreDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = ExceptionMessage.STORE_NOTFOUND,
                    StatusCode = HttpStatusCode.NotFound
                };

            var res = await BuildToStoreDetailsResponseDTO(store);

            var provinces = await _ghtkService.SyncProvincesAsync();
            var province = provinces.FirstOrDefault(p => p.Id == res.ProvinceId);
            res.ProvinceName = province?.Name ?? "Not found";

            var districts = await _ghtkService.SyncDistrictsAsync(res.ProvinceId);
            var district = districts.FirstOrDefault(d => d.Id == res.DistrictId);
            res.DistrictName = district?.Name ?? "Not found";

            var wards = await _ghtkService.SyncWardsAsync(res.DistrictId);
            var ward = wards.FirstOrDefault(w => w.Id == res.WardId);
            res.WardName = ward?.Name ?? "Not found";

            var list_addressCustomer = await _ghtkService.SyncAddressAsync(res.WardId);
            var addressNameCustomer = list_addressCustomer.FirstOrDefault(w => w.Id == res.AddressId);
            res.AddressName = addressNameCustomer?.Name ?? "Not Found";

            return ResponseService<StoreDetailsResponseDTO>.OK(res);
        }

        private async Task<ResponseDTO> CreateOrUpdateStoreAttachments(int storeId, List<StoreAttachmentRequestDTO>? payload)
        {
            var store = _storeRepository.GetById(storeId);

            List<StoreAttachment>? dbList = store?.StoreAttachmentsNavigation?.ToList();

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
            var response = await CreateOrUpdateStoreByLoginUser(userId, payload);

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

        public async Task<ResponseDTO> CheckStoreExistByContactInformation(StoreRequestDTO payload, int? excludeStoreId)
        {
            var isExistContactNumber = unitOfWork.StoreRepository.Search(
                item => item.ContactNumber == payload.ContactNumber && item.Id != excludeStoreId)
                .Any();

            if (isExistContactNumber)
                return ResponseService<object>.BadRequest("Your contact number is already used. Please try another number");

            return ResponseService<object>.OK(null);
        }

        public async Task<GenericResponseDTO<StoreDetailsResponseDTO>> CreateOrUpdateStoreByLoginUser(int userId, StoreRequestDTO payload)
        {
            int? loginUserId = _userService.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<StoreDetailsResponseDTO>.Unauthorize("Please login to update Store");

            User user = await _userRepository.GetUserById((int)loginUserId);

            if (!(user?.UserRoles?.Count(ur => ur.RoleId == (int)RoleEnum.STORE) > 0))
                return ResponseService<StoreDetailsResponseDTO>.BadRequest(ExceptionMessage.INVALID_STORE_ROLE);

            if (user == null)
                return ResponseService<StoreDetailsResponseDTO>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);

            Store? store = _storeRepository.GetByUserId(user.Id);

            var checkExist = await CheckStoreExistByContactInformation(payload, store?.Id);

            if (!checkExist.IsSuccess)
                return ResponseService<StoreDetailsResponseDTO>.BadRequest(checkExist.Message);

            store = store == null ? new Store
            {
                IsActive = 2 //Default is pending
            } : store;

            var provinces = await _ghtkService.SyncProvincesAsync();
            if (!provinces.Any(p => p.Id == payload.ProvinceId))
                return ResponseService<StoreDetailsResponseDTO>.BadRequest("Invalid Province ID");

            var districts = await _ghtkService.SyncDistrictsAsync(payload.ProvinceId);
            if (!districts.Any(d => d.Id == payload.DistrictId))
                return ResponseService<StoreDetailsResponseDTO>.BadRequest("Invalid District ID");

            var wards = await _ghtkService.SyncWardsAsync(payload.DistrictId);
            if (!wards.Any(w => w.Id == payload.WardId))
                return ResponseService<StoreDetailsResponseDTO>.BadRequest("Invalid Ward ID");

            var list_addressCustomer = await _ghtkService.SyncAddressAsync(payload.WardId);
            var addressNameCustomer = list_addressCustomer.FirstOrDefault(w => w.Id == payload.AddressId);
            if (addressNameCustomer == null)
                return ResponseService<StoreDetailsResponseDTO>.BadRequest("Invalid Ward Address ID");

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
            store.WardId = payload.WardId;
            store.DistrictId = payload.DistrictId;
            store.ProvinceId = payload.ProvinceId;
            store.AddressId = payload.AddressId;
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
            Store? store = _storeRepository.GetByUserId(userId);

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
                includeProperties: "Owner",
                pageIndex: paginationRequest.PageIndex,
                pageSize: paginationRequest.PageSize
            );

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = HttpStatusCode.OK,
                Data = PaginationMapper<Store, StoreResponseDTO>.MapTo(StoreMapper.MapToStoreResponse, res)
            };
        }

        public async Task<ResponseDTO> CheckValidBusinessLicenseRequest(StoreBusinessLicensesDTO payload, int? exludeId = null)
        {
            var checkExistBusinessNumber = businessLicenseRepository.Search(
                item => item.LiscenseNumber.CompareTo(payload.LiscenseNumber) == 0 && item.Id != exludeId
            ).Any();

            if (checkExistBusinessNumber)
                return ResponseService<object>.BadRequest("Your business license is already used. Please try again");

            return ResponseService<object>.OK(null);
        }

        public async Task<GenericResponseDTO<BusinessLicenses>> CreateOrUpdateBusinessLicences(StoreBusinessLicensesDTO payload)
        {
            var loginUserId = _userService.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<BusinessLicenses>.Unauthorize("Please login to update license");

            var store = _storeRepository.GetByUserId((int)loginUserId);

            if (store == null)
                return ResponseService<BusinessLicenses>.NotFound(ExceptionMessage.STORE_NOTFOUND);

            int storeId = store.Id;

            var businessLicense = businessLicenseRepository.GetByStoreId(storeId);

            businessLicense = businessLicense == null ? new BusinessLicenses() : businessLicense;

            businessLicense.BackIdentification = payload.BackIdentification;
            businessLicense.FrontIdentification = payload.FrontIdentification;
            businessLicense.BusinessLicences = payload.BusinessLicences;
            businessLicense.LiscenseNumber = payload.LiscenseNumber;
            businessLicense.storeId = payload.StoreId;
            businessLicense.IssueDate = payload.IssueDate;
            businessLicense.ExpiredDate = payload.ExpiredDate;
            businessLicense.IssueBy = payload.IssueBy;

            var checkedExist = await CheckValidBusinessLicenseRequest(payload, businessLicense.Id);

            if (!checkedExist.IsSuccess)
                return ResponseService<BusinessLicenses>.BadRequest(checkedExist.Message);

            try
            {
                if (businessLicense.Id > 0) //Update
                    businessLicense = businessLicenseRepository.Update(businessLicense);
                else //Create
                    businessLicense = businessLicenseRepository.Create(businessLicense);

                return ResponseService<BusinessLicenses>.OK(businessLicense);

            } catch (Exception ex)
            {
                return ResponseService<BusinessLicenses>.BadRequest(ex.Message);
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

        public async Task<GenericResponseDTO<GetStoreDetailsResponseDTO>> GetPaginationStoresDetails(PaginationRequest paginationRequest, int storeId)
        {
            var store = await _storeRepository.GetProductByIdAsync(storeId);

            if (store == null)
                return ResponseService<GetStoreDetailsResponseDTO>.NotFound("Store doesn't exist");

            var ownerId = store.OwnerId;

            // Tính tổng số phần tử cần lấy cho mỗi loại
            int halfPageSize = paginationRequest.PageSize / 2;

            // Phân trang danh sách sản phẩm IoT
            var paginatedDevices = unitOfWork.IotsDeviceRepository.GetPaginate(
                filter: d => d.CreatedBy == ownerId,
                pageIndex: paginationRequest.PageIndex,
                pageSize: halfPageSize
            );

            // Phân trang danh sách Combo
            var paginatedCombos = unitOfWork.ComboRepository.GetPaginate(
                filter: c => c.CreatedBy == ownerId,
                pageIndex: paginationRequest.PageIndex,
                pageSize: paginationRequest.PageSize - halfPageSize // Đảm bảo tổng đúng với PageSize
            );

            // Chuyển đổi sang DTO
            var responseDTO = new GetStoreDetailsResponseDTO
            {
                StoreId = store.Id,
                StoreName = store.Name,
                OwnerId = store.OwnerId,
                OwnerName = store.Owner.Fullname,
                DevicesIot = paginatedDevices.Data.Select(device => new IotDeviceByStoreDetailsResponseDTO
                {
                    Id = device.Id,
                    Name = device.Name,
                    Description = device.Description,
                    Price = device.Price,
                    ImageUrl = device.ImageUrl,
                    Rating = device.Rating,
                    CreatedDate = device.CreatedDate
                }).ToList(),
                Combos = paginatedCombos.Data.Select(combo => new ComboByStoreDetailsResponseDTO
                {
                    Id = combo.Id,
                    Name = combo.Name,
                    Description = combo.Description,
                    Price = combo.Price,
                    ImageUrl = combo.ImageUrl,
                    Rating = combo.Rating,
                    CreatedDate = combo.CreatedDate
                }).ToList(),
                TotalDevices = paginatedDevices.TotalCount,
                TotalCombos = paginatedCombos.TotalCount
            };

            return ResponseService<GetStoreDetailsResponseDTO>.OK(responseDTO);
        }

    }
}
