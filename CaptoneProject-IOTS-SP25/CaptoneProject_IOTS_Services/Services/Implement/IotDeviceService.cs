using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class IotDeviceService : IIotDevicesService
    {
        private readonly IAttachmentsService attachmentsService;
        private readonly IUserServices userServices;
        private readonly UnitOfWork unitOfWork;
        public IotDeviceService(IAttachmentsService attachmentsService,
            IUserServices userServices,
            UnitOfWork unitOfWork)
        {
            this.attachmentsService = attachmentsService;
            this.userServices = userServices;
            this.unitOfWork = unitOfWork;
        }

        private string GetApplicationSerialNumber(int storeId, string serialNumber, int deviceType)
        {
            return "DV{StoreId}{DeviceType}{SerialNumber}"
                .Replace("{StoreId}", storeId.ToString())
                .Replace("{DeviceType}", (deviceType == (int)IotDeviceTypeEnum.NEW ? "N" : "O"))
                .Replace("{SerialNumber}", serialNumber);
        }

        public async Task<bool> CreateOrUpdateDeviceSpecification(int deviceId, List<DeviceSpecificationDTO>? request)
        {
            var dbSpecList = unitOfWork.DeviceSpecificationRepository.GetDeviceSpecificationByDeviceId(deviceId);

            if (dbSpecList != null)
                await unitOfWork.DeviceSpecificationRepository.RemoveAsync(dbSpecList);

            if (request != null)
            {
                List<DeviceSpecificationsItem> saveSpecItems = new List<DeviceSpecificationsItem>();
                request.ForEach(item =>
                {
                    var saveSpec = new DeviceSpecification
                    {
                        CreatedDate = DateTime.Now,
                        IotDeviceId = deviceId,
                        Name = item.Name
                    };

                    saveSpec = unitOfWork.DeviceSpecificationRepository.Create(saveSpec);

                    var specItemsList = item?.DeviceSpecificationItemsList?.Select(specItem => new DeviceSpecificationsItem
                    {
                        CreatedDate = DateTime.Now,
                        SpecificationProperty = specItem?.SpecificationProperty,
                        SpecificationValue = specItem?.SpecificationValue,
                    });

                    if (specItemsList != null)
                        saveSpecItems.AddRange(specItemsList);
                });

                if (saveSpecItems != null)
                    await unitOfWork.DeviceSpecificationItemRepository.CreateAsync(saveSpecItems);
            }

            return true;
        }

        public async Task<GenericResponseDTO<IotDeviceDetailsDTO>> CreateOrUpdateIotDevice(int? id, CreateUpdateIotDeviceDTO payload)
        {
            var loginUser = userServices.GetLoginUser();

            if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.STORE))
                return ResponseService<IotDeviceDetailsDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var saveDevice = (id == null) ? new IotsDevice() : unitOfWork.IotsDeviceRepository.GetById((int)id);
            decimal? dbRating = saveDevice.Rating;

            if (saveDevice == null)
                return ResponseService<IotDeviceDetailsDTO>.NotFound("Iot device cannot be found. Please try again");

            var createdDate = saveDevice.CreatedDate;
            var createdBy = (saveDevice.CreatedBy == null) ? loginUserId : saveDevice.CreatedBy;

            if (id != null && saveDevice.CreatedBy != loginUserId)
                return ResponseService<IotDeviceDetailsDTO>.Unauthorize("You don't have permission to access");
            int? storeId = id == null ? unitOfWork.StoreRepository.GetByUserId(loginUserId)?.Id : saveDevice.StoreId;
            
            if (storeId == null)
            {
                return ResponseService<IotDeviceDetailsDTO>.BadRequest("Cannot found your store information");
            }

            saveDevice = IotDeviceMapper.MapToIotsDevice(payload);

            saveDevice.CreatedDate = createdDate;
            saveDevice.UpdatedDate = DateTime.Now;
            saveDevice.CreatedBy = createdBy;
            saveDevice.UpdatedBy = loginUserId;
            saveDevice.Id = (id == null) ? saveDevice.Id : (int)id;
            saveDevice.StoreId = (int)storeId;
            saveDevice.ApplicationSerialNumber = GetApplicationSerialNumber((int)storeId, payload.SerialNumber, (int)payload.DeviceType);
            saveDevice.Rating = dbRating;

            var checkExistRecord = unitOfWork.IotsDeviceRepository.GetByApplicationSerialNumber(saveDevice.ApplicationSerialNumber);

            //Existing application serial number
            if (checkExistRecord != null && checkExistRecord.Id != saveDevice.Id)
            {
                return ResponseService<IotDeviceDetailsDTO>.BadRequest("The Serial Number was duplicated in your Store. Please enter another Serial Number");
            }

            try
            {
                if (id > 0) //Update
                    saveDevice = unitOfWork.IotsDeviceRepository.Update(saveDevice);
                else
                    saveDevice = unitOfWork.IotsDeviceRepository.Create(saveDevice);

                //Create or update attachments
                var res = await attachmentsService.CreateOrUpdateAttachments(saveDevice.Id,
                    (int)EntityTypeEnum.IOT_DEVICE,
                    payload.Attachments);

                await CreateOrUpdateDeviceSpecification(saveDevice.Id, payload.DeviceSpecificationsList);

                return await GetIotDeviceById(saveDevice.Id);
            } catch (Exception e)
            {
                //return ResponseService<IotDeviceDetailsDTO>.BadRequest(e.Message);
                return ResponseService<IotDeviceDetailsDTO>.BadRequest("Cannot save the IOT Device Information. Please try again");
            }
        }

        public async Task<GenericResponseDTO<IotDeviceDetailsDTO>> GetIotDeviceById(int id)
        {
            try
            {
                var device = unitOfWork.IotsDeviceRepository.GetById(id);
                
                if (device == null)
                    return ResponseService<IotDeviceDetailsDTO>.NotFound("Iot device cannot be found. Please try again");

                var loginUserId = userServices.GetLoginUserId();

                bool isEdit = loginUserId == null ? false : unitOfWork.StoreRepository
                                                                .GetByUserId((int)loginUserId)?
                                                                .Id == device?.StoreId;

                var res = IotDeviceMapper.MapToIotDeviceDetailsDTO(device);
                
                res.IsEdit = isEdit;

                if (res == null)
                    return ResponseService<IotDeviceDetailsDTO>.NotFound("Iot device cannot be found. Please try again");

                var attachments = await attachmentsService.GetByEntityId(res.Id, (int)EntityTypeEnum.IOT_DEVICE);

                res.Attachments = attachments;

                return ResponseService<IotDeviceDetailsDTO>.OK(res);
            }
            catch (Exception ex)
            {
                return ResponseService<IotDeviceDetailsDTO>.BadRequest("Cannot get iot device information. Please try again");
            }
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<IotDeviceItem>>> GetPagination(int? filterStoreId, int? categoryFilterId, IotDeviceTypeEnum? deviceTypeFilter, PaginationRequest payload)
        {
            int? loginUserId = userServices.GetLoginUserId();
            var isStore = await userServices.CheckLoginUserRole(RoleEnum.STORE);
            var isAnonymousOrCustomer = (loginUserId == null) || await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER);

            int? loginStoreId = isStore && loginUserId != null ? unitOfWork?.StoreRepository?.GetByUserId((int)loginUserId)?.Id : null;

            var res = unitOfWork?.IotsDeviceRepository.GetPaginate(
                filter: item => (item.Name.Contains(payload.SearchKeyword))
                    && ((isStore && item.StoreId == loginStoreId) || !isStore)
                    && ((isAnonymousOrCustomer && item.IsActive > 0) || !isAnonymousOrCustomer)
                    && (filterStoreId == null || filterStoreId == item.StoreId)
                    && ((categoryFilterId == null) || (item.CategoryId == categoryFilterId))
                    && (deviceTypeFilter == null || (int)deviceTypeFilter == item.DeviceType),
                orderBy: ob => ob.OrderByDescending(item => item.Rating),
                includeProperties: "StoreNavigation,Category",
                pageIndex: payload.PageIndex,
                pageSize: payload.PageSize
            );

            res = res == null ? new PaginationResponseDTO<IotsDevice>() : res;

            return ResponseService<PaginationResponseDTO<IotDeviceItem>>.OK(
                PaginationMapper<IotsDevice, IotDeviceItem>.MappingTo((res) => new IotDeviceItem
                {
                    Id = res.Id,
                    DeviceType = res.DeviceType,
                    DeviceTypeLabel = (res.DeviceType == 1) ? "New" : "Second-hand",
                    CategoryId = res.CategoryId,
                    CategoryName = res.Category.Label,
                    ImageUrl = res.ImageUrl,
                    Name = res.Name,
                    Price = res.Price,
                    Quantity = res.Quantity,
                    SecondHandPrice = res.SecondHandPrice,
                    SecondhandQualityPercent = res.SecondhandQualityPercent,
                    StoreId = res.StoreId,
                    StoreNavigationName = res.StoreNavigation.Name,
                    StoreNavigationImageUrl = res.StoreNavigation.ImageUrl,
                    Summary = res.Summary,
                    IsActive = res.IsActive
                }, res)
            );
        }

        public async Task<GenericResponseDTO<IotDeviceDetailsDTO>> UpdateIotDeviceStatus(int id, int status)
        {
            var loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<IotDeviceDetailsDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var isAdmin = await userServices.CheckLoginUserRole(RoleEnum.ADMIN);
            var isManager = await userServices.CheckLoginUserRole(RoleEnum.MANAGER);
            var isStore = await userServices.CheckLoginUserRole(RoleEnum.STORE);

            var device = unitOfWork.IotsDeviceRepository.GetById(id);

            if (device == null)
                return ResponseService<IotDeviceDetailsDTO>.NotFound(ExceptionMessage.DEVICE_NOTFOUND);

            if (isAdmin || isManager || isStore || device.CreatedBy == loginUserId)
            {
                try
                {
                    device.IsActive = (status > 0) ? 1 : 0;
                    device.UpdatedBy = loginUserId;
                    device.UpdatedDate = DateTime.Now;

                    device = unitOfWork.IotsDeviceRepository.Update(device);
                } catch
                {
                    return ResponseService<IotDeviceDetailsDTO>.BadRequest("Cannot Update the Iot Device");
                }
            }
            
            return ResponseService<IotDeviceDetailsDTO>.Unauthorize(ExceptionMessage.DEVICE_UPDATE_SUCCESS);
        }
    }
}
