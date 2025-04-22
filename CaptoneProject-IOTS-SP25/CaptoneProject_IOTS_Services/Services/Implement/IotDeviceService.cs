﻿using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
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
        private readonly IStoreService storeService;
        private readonly UnitOfWork unitOfWork;
        public IotDeviceService(IAttachmentsService attachmentsService,
            IUserServices userServices,
            UnitOfWork unitOfWork,
            IStoreService storeService)
        {
            this.attachmentsService = attachmentsService;
            this.userServices = userServices;
            this.unitOfWork = unitOfWork;
            this.storeService = storeService;
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
                        DeviceSpecificationId = saveSpec.Id
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
                {
                    // Check duplicate label within the same store
                    bool isDuplicate = unitOfWork.IotsDeviceRepository.Any(x =>
                        x.StoreId == payload.StoreId &&
                        x.Name.Trim().ToLower() == payload.Name.Trim().ToLower());

                    if (isDuplicate)
                    {
                        return ResponseService<IotDeviceDetailsDTO>.BadRequest("IOT Devices already exists in this store.");
                    }

                    saveDevice = unitOfWork.IotsDeviceRepository.Create(saveDevice);
                }
                    
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

                var storeDetails = await storeService.BuildToStoreDetailsResponseDTO(device.StoreNavigation);

                res.StoreInfo = storeDetails;

                res.IsEdit = isEdit;

                if (res == null)
                    return ResponseService<IotDeviceDetailsDTO>.NotFound("Iot device cannot be found. Please try again");

                var attachments = await attachmentsService.GetByEntityId(res.Id, (int)EntityTypeEnum.IOT_DEVICE);

                res.Attachments = attachments;

                int iotId = res.Id;

                var ratingQuery = unitOfWork?.FeedbackRepository.Search(
                    item => iotId == item.OrderItem.IosDeviceId
                ).Include(item => item.OrderItem);

                res.Rating = ((ratingQuery?.Sum(r => r.Rating) ?? 0) + ProductConst.DEFAULT_RATING) / ((ratingQuery?.Count() ?? 0) + 1);

                return ResponseService<IotDeviceDetailsDTO>.OK(res);
            }
            catch (Exception ex)
            {
                return ResponseService<IotDeviceDetailsDTO>.BadRequest("Cannot get iot device information. Please try again");
            }
        }

        private IotDeviceItem BuildIotDeviceItem(IotsDevice source)
        {
            var res = GenericMapper<IotsDevice, IotDeviceItem>.MapTo(source);
            res.CategoryName = source.Category.Label;
            res.DeviceTypeLabel = (res.DeviceType == 1) ? "New" : "Second-hand";

            return res;
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<IotDeviceItem>>> GetPagination(int? filterStoreId, int? categoryFilterId, IotDeviceTypeEnum? deviceTypeFilter, PaginationRequest payload)
        {
            int? loginUserId = userServices.GetLoginUserId();
            var role = userServices.GetRole();

            var isStore = role == (int)RoleEnum.STORE;
            var isAdminOrManager = (role == (int)RoleEnum.ADMIN || role == (int)RoleEnum.MANAGER);
            var isAnonymousOrCustomer = (loginUserId == null) || role == (int)RoleEnum.CUSTOMER;

            var res = unitOfWork?.IotsDeviceRepository.GetPaginate(
                filter: item => item.Name.Contains(payload.SearchKeyword)
                    && (filterStoreId == null || filterStoreId == item.StoreId)
                    && ((categoryFilterId == null) || (item.CategoryId == categoryFilterId))
                    && (deviceTypeFilter == null || (int)deviceTypeFilter == item.DeviceType)
                    && (isAdminOrManager || (isStore && item.CreatedBy == loginUserId) || (isAnonymousOrCustomer && item.IsActive > 0)),
                orderBy: ob => ob.OrderByDescending(item => item.CreatedDate),
                includeProperties: "StoreNavigation,Category",
                pageIndex: payload.PageIndex,
                pageSize: payload.PageSize
            );

            res = res == null ? new PaginationResponseDTO<IotsDevice>() : res;

            var idsList = res?.Data?.Select(iot => iot.Id).ToList();

            var ratingQuery = unitOfWork?.FeedbackRepository.Search(
                item => idsList.Any(id => id == item.OrderItem.IosDeviceId)
            ).Include(item => item.OrderItem);

            res.Data = res?.Data?.Select(
                iot =>
                {
                    var ratingList = ratingQuery?.Where(r => r.OrderItem.IosDeviceId == iot.Id);

                    iot.Rating = ((ratingList?.Sum(r => r.Rating) ?? 0) + ProductConst.DEFAULT_RATING) / ((ratingList?.Count() ?? 0) + 1);

                    return iot;
                }
            );

            return ResponseService<PaginationResponseDTO<IotDeviceItem>>.OK(
                PaginationMapper<IotsDevice, IotDeviceItem>.MapTo(BuildIotDeviceItem, res)
            );
        }

        public async Task<GenericResponseDTO<IotDeviceDetailsUpdateDTO>> UpdateIotDeviceStatus(int id, int status)
        {
            var loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<IotDeviceDetailsUpdateDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var isAdmin = await userServices.CheckLoginUserRole(RoleEnum.ADMIN);
            var isManager = await userServices.CheckLoginUserRole(RoleEnum.MANAGER);
            var isStore = await userServices.CheckLoginUserRole(RoleEnum.STORE);

            var device = unitOfWork.IotsDeviceRepository.GetById(id);

            if (device == null)
                return ResponseService<IotDeviceDetailsUpdateDTO>.NotFound(ExceptionMessage.DEVICE_NOTFOUND);

            if (isAdmin || isManager || isStore || device.CreatedBy == loginUserId)
            {
                try
                {
                    device.IsActive = (status > 0) ? 1 : 0;
                    device.UpdatedBy = loginUserId;
                    device.UpdatedDate = DateTime.Now;

                    unitOfWork.IotsDeviceRepository.Update(device);

                    // Chuẩn bị response DTO
                    var responseDto = new IotDeviceDetailsUpdateDTO
                    {
                        Id = device.Id,
                        Name = device.Name,
                        IsActive = device.IsActive,
                        CategoryName = device.Category.Label,
                        ImageUrl = device.ImageUrl,
                        Price = device.Price,
                        Quantity = device.Quantity,
                        UpdatedBy = device.UpdatedBy,
                        UpdatedDate = device.UpdatedDate
                    };

                    var notiMessage = "";

                    if (status == 0 && device.CreatedBy != loginUserId)
                    {
                        notiMessage = $"Your device '{device.Name}' has been deactivated by Manager. Please check the reason";
                    } else if (status > 0 && device.CreatedBy != loginUserId)
                    {
                        notiMessage = $"Your device '{device.Name}' has been activated by Manager";
                    }

                    if (notiMessage != "")
                    {
                        var noti = new Notifications
                        {
                            Content = notiMessage,
                            Title = notiMessage,
                            EntityId = device.Id,
                            EntityType = (int)EntityTypeEnum.IOT_DEVICE,
                            ReceiverId = (int)device.CreatedBy
                        };

                        _ = unitOfWork.NotificationRepository.Create(noti);
                    }

                    return ResponseService<IotDeviceDetailsUpdateDTO>.OK(responseDto);
                }
                catch (Exception ex)
                {
                    return ResponseService<IotDeviceDetailsUpdateDTO>.BadRequest("Cannot update the IoT device. Error: " + ex.Message);
                }
            }

            return ResponseService<IotDeviceDetailsUpdateDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);
        }
    }
}
