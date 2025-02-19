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
        private readonly IotsDeviceRepository iotsDeviceRepository;
        private readonly IAttachmentsService attachmentsService;
        private readonly IUserServices userServices;
        private readonly StoreRepository storeRepository;

        public IotDeviceService(IotsDeviceRepository iotsDeviceRepository, 
            IAttachmentsService attachmentsService,
            IUserServices userServices,
            StoreRepository storeRepository)
        {
            this.iotsDeviceRepository = iotsDeviceRepository;
            this.attachmentsService = attachmentsService;
            this.userServices = userServices;
            this.storeRepository = storeRepository;
        }

        private string GetApplicationSerialNumber(int storeId, string serialNumber, int deviceType)
        {
            return "DV{StoreId}{DeviceType}{SerialNumber}"
                .Replace("{StoreId}", storeId.ToString())
                .Replace("{DeviceType}", (deviceType == (int)IotDeviceTypeEnum.NEW ? "N" : "O"))
                .Replace("{SerialNumber}", serialNumber);
        }
        public async Task<GenericResponseDTO<IotDeviceDetailsDTO>> CreateOrUpdateIotDevice(int? id, CreateUpdateIotDeviceDTO payload)
        {
            var loginUser = userServices.GetLoginUser();

            if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.STORE))
                return ResponseService<IotDeviceDetailsDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var saveDevice = (id == null) ? new IotsDevice() : iotsDeviceRepository.GetById((int)id);

            if (saveDevice == null)
                return ResponseService<IotDeviceDetailsDTO>.NotFound("Iot device cannot be found. Please try again");

            var createdDate = saveDevice.CreatedDate;
            var createdBy = (saveDevice.CreatedBy == null) ? loginUserId : saveDevice.CreatedBy;

            if (id != null && saveDevice.CreatedBy != loginUserId)
                return ResponseService<IotDeviceDetailsDTO>.Unauthorize("You don't have permission to access");
            int? storeId = id == null ? storeRepository.GetByUserId(loginUserId)?.Id : saveDevice.StoreId;
            
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

            var checkExistRecord = iotsDeviceRepository.GetByApplicationSerialNumber(saveDevice.ApplicationSerialNumber);

            //Existing application serial number
            if (saveDevice.Id > 0 && checkExistRecord != null && checkExistRecord.Id != saveDevice.Id)
            {
                return ResponseService<IotDeviceDetailsDTO>.BadRequest("The Serial Number was duplicated in your Store. Please enter another Serial Number");
            }

            try
            {
                if (id > 0) //Update
                    saveDevice = iotsDeviceRepository.Update(saveDevice);
                else
                    saveDevice = iotsDeviceRepository.Create(saveDevice);

                var res = await attachmentsService.CreateOrUpdateAttachments(saveDevice.Id,
                    (int)EntityTypeEnum.IOT_DEVICE,
                    payload.Attachments);

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
                var device = iotsDeviceRepository.GetById(id);

                var res = IotDeviceMapper.MapToIotDeviceDetailsDTO(device);

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

        public async Task<GenericResponseDTO<PaginationResponseDTO<IotDeviceItem>>> GetPagination(PaginationRequest payload)
        {
            int? loginUserId = userServices.GetLoginUserId();
            var isStore = await userServices.CheckLoginUserRole(RoleEnum.STORE);
            var isAnonymousOrCustomer = (loginUserId == null) || await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER);

            var res = iotsDeviceRepository.GetPaginate(
                filter: item => (item.Name.Contains(payload.SearchKeyword))
                    && ((isStore && item.CreatedBy == loginUserId) || !isStore)
                    && ((isAnonymousOrCustomer && item.IsActive > 0) || !isAnonymousOrCustomer),
                orderBy: null,
                includeProperties: "StoreNavigation,Category",
                pageIndex: payload.PageIndex,
                pageSize: payload.PageSize
            );

            return ResponseService<PaginationResponseDTO<IotDeviceItem>>.OK(
                PaginationMapper<IotsDevice, IotDeviceItem>.MappingTo((res) => new IotDeviceItem
                {
                    Id = res.Id,
                    DeviceType = res.DeviceType,
                    DeviceTypeLabel = (res.DeviceType == 1) ? "New" : "Second Hand",
                    Category = res.Category,
                    ImageUrl = res.ImageUrl,
                    Name = res.Name,
                    Price = res.Price,
                    Quantity = res.Quantity,
                    SecondHandPrice = res.SecondHandPrice,
                    SecondhandQualityPercent = res.SecondhandQualityPercent,
                    StoreNavigation = res.StoreNavigation,
                    Summary = res.Summary
                }, res)
            );
        }

        public async Task<GenericResponseDTO<IotDeviceDetailsDTO>> UpdateIotDeviceStatus(int id, int status)
        {
            var loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<IotDeviceDetailsDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var isAdminOrManager = await userServices.CheckLoginUserRole(RoleEnum.MANAGER) || await userServices.CheckLoginUserRole(RoleEnum.ADMIN);

            var device = iotsDeviceRepository.GetById(id);

            if (device == null)
                return ResponseService<IotDeviceDetailsDTO>.NotFound("The Iot Device cannot be found");

            if (isAdminOrManager || device.CreatedBy == loginUserId)
            {
                try
                {
                    device.IsActive = (status > 0) ? 1 : 0;
                    device.UpdatedBy = loginUserId;
                    device.UpdatedDate = DateTime.Now;

                    device = iotsDeviceRepository.Update(device);
                } catch
                {
                    return ResponseService<IotDeviceDetailsDTO>.BadRequest("Cannot Update the Iot Device");
                }
            }
            
            return ResponseService<IotDeviceDetailsDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);
        }
    }
}
