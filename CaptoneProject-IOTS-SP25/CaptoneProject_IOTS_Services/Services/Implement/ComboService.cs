using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.OData.ModelBuilder.Capabilities.V1;
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
    public class ComboService : IComboService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
        private readonly IAttachmentsService attachmentsService;

        public ComboService(UnitOfWork unitOfWork, IUserServices userService, IAttachmentsService attachmentsService)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userService;
            this.attachmentsService = attachmentsService;
        }
        private string GetApplicationSerialNumber(int storeId, string serialNumber)
        {
            return "CB{StoreId}{DeviceType}{SerialNumber}"
                .Replace("{StoreId}", storeId.ToString())
                .Replace("{DeviceType}", "N")
                .Replace("{SerialNumber}", serialNumber);
        }

        public async Task<PaginationResponseDTO<ComboItemDTO>> GetPaginationCombos(PaginationRequest request)
        {
            int? loginUserId = userServices.GetLoginUserId();
            var isStore = await userServices.CheckLoginUserRole(RoleEnum.STORE);
            var isAnonymousOrCustomer = (loginUserId == null) || await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER);

            int? loginStoreId = isStore && loginUserId != null ? unitOfWork?.StoreRepository?.GetByUserId((int)loginUserId)?.Id : null;

            var pagination = unitOfWork?.ComboRepository.GetPaginate(
                filter: item => (item.Name.Contains(request.SearchKeyword))
                    && ((isStore && item.StoreId == loginStoreId) || !isStore)
                    && ((isAnonymousOrCustomer && item.IsActive > 0) || !isAnonymousOrCustomer),
                orderBy: ob => ob.OrderByDescending(item => item.Rating),
                includeProperties: "StoreNavigation",
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            var res = PaginationMapper<Combo, ComboItemDTO>.MappingTo((item) =>
            {
                return new ComboItemDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsActive = item.IsActive,
                    ApplicationSerialNumber = item.ApplicationSerialNumber,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    UpdateDate = item.UpdateDate,
                    UpdatedBy = item.UpdatedBy,
                    ImageUrl = item.ImageUrl,
                    Price = item.Price,
                    StoreId = item.StoreId,
                    StoreNavigationName = item?.StoreNavigation?.Name,
                    Rating = item?.Rating,
                    Summary = item?.Summary
                };
            }, pagination);

            return res;
        }

        public async Task<ComboDetailsResponseDTO> GetComboDetailsById(int comboId)
        {
            var combo = unitOfWork.ComboRepository.GetById(comboId);

            if (combo == null)
                throw new Exception("Combo cannot be found. Please try again");

            var res = ComboMapper.MapToComboDetailsResponseDTO(combo);

            var attachments = await attachmentsService.GetByEntityId(comboId, (int)EntityTypeEnum.IOT_DEVICE_COMBO);

            var deviceComboList = unitOfWork.DeviceComboRepository.GetItemsByComboId(comboId);

            res.AttachmentsList = attachments;

            if (deviceComboList != null)
                res.DeviceComboList = deviceComboList.Select(item => new DeviceComboReponseDTO
                {
                    Amount = item.Amount,
                    DeviceComboId = item.Id,
                    IotDeviceId = item.IotDeviceId,
                    DeviceName = item.IotDeviceNavigation.Name,
                    DeviceSummary = item.IotDeviceNavigation.Summary,
                    OriginalPrice = item.IotDeviceNavigation.Price
                }).ToList();

            return res;
        }

        private async Task<bool> CreateOrUpdateIotDevicesCombo(int comboId, List<CreateUpdateDeviceComboDTO> request)
        {
            var dbList = unitOfWork.DeviceComboRepository.Search(item => item.ComboId == comboId).ToList();

            var removeList = dbList?.Where(i => request.Count(item => item.IotDeviceId == i.IotDeviceId) <= 0)?.ToList();

            if (removeList != null)
                await unitOfWork.DeviceComboRepository.RemoveAsync(removeList);

            //Update Quantity
            var updatedQuantityList = request.Where(item => item.DeviceComboId > 0).Select(item => new IotsDevicesCombo
            {
                Amount = item.Amount,
                ComboId = comboId,
                CreatedDate = DateTime.Now,
                IotDeviceId = item.IotDeviceId,
                Id = item.DeviceComboId
            }).ToList();

            if (updatedQuantityList != null && updatedQuantityList.Count > 0)
                await unitOfWork.DeviceComboRepository.UpdateAsync(updatedQuantityList);

            //Create new 
            var createdNewList = request.Where(item => item.DeviceComboId <= 0).Select(item => new IotsDevicesCombo
            {
                Amount = item.Amount,
                ComboId = comboId,
                CreatedDate = DateTime.Now,
                IotDeviceId = item.IotDeviceId,
                Id = item.DeviceComboId
            }).ToList();

            if (createdNewList != null && createdNewList.Count > 0)
                await unitOfWork.DeviceComboRepository.CreateAsync(createdNewList);

            return true;
        }

        private bool ValidateRequest(CreateUpdateComboDTO payload)
        {
            var distinctList = payload.DeviceComboList.Select(item => item.IotDeviceId).Distinct();

            if (distinctList.Count() != payload.DeviceComboList.Count())
                throw new Exception("Duplicated iot devices in Combo. Please check and try again");

            return true;
        }

        public async Task<GenericResponseDTO<ComboDetailsResponseDTO>> CreateOrUpdateCombo(int? id, CreateUpdateComboDTO payload)
        {
            var loginUser = userServices.GetLoginUser();

            if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.STORE))
                return ResponseService<ComboDetailsResponseDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var validationOk = ValidateRequest(payload);

            var saveCombo = (id == null) ? new Combo() : unitOfWork.ComboRepository.GetById((int)id);
            decimal? dbRating = saveCombo?.Rating == null ? 4 : saveCombo?.Rating;

            if (saveCombo == null)
                return ResponseService<ComboDetailsResponseDTO>.NotFound("Combo cannot be found. Please try again");

            var createdDate = saveCombo.CreatedDate;
            var createdBy = (saveCombo.CreatedBy == null) ? loginUserId : saveCombo.CreatedBy;

            if (id != null && saveCombo.CreatedBy != loginUserId)
                return ResponseService<ComboDetailsResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);
            
            int? storeId = id == null ? unitOfWork.StoreRepository.GetByUserId(loginUserId)?.Id : saveCombo.StoreId;

            if (storeId == null)
            {
                return ResponseService<ComboDetailsResponseDTO>.BadRequest("Cannot found your store information");
            }

            saveCombo = ComboMapper.MapToCombo(payload);

            saveCombo.CreatedDate = createdDate;
            saveCombo.UpdateDate = DateTime.Now;
            saveCombo.CreatedBy = createdBy;
            saveCombo.UpdatedBy = loginUserId;
            saveCombo.Id = (id == null) ? saveCombo.Id : (int)id;
            saveCombo.StoreId = (int)storeId;
            saveCombo.ApplicationSerialNumber = GetApplicationSerialNumber((int)storeId, payload.SerialNumber);
            saveCombo.Rating = dbRating;

            var checkExistRecord = unitOfWork.ComboRepository.GetByApplicationSerialNumber(saveCombo.ApplicationSerialNumber);

            //Existing application serial number
            if (checkExistRecord != null && checkExistRecord.Id != saveCombo.Id)
            {
                return ResponseService<ComboDetailsResponseDTO>.BadRequest("The Serial Number was duplicated in your Store. Please enter another Serial Number");
            }

            try
            {
                if (id > 0) //Update
                    saveCombo = unitOfWork.ComboRepository.Update(saveCombo);
                else
                    saveCombo = unitOfWork.ComboRepository.Create(saveCombo);

                var res = await attachmentsService.CreateOrUpdateAttachments(saveCombo.Id,
                    (int)EntityTypeEnum.IOT_DEVICE_COMBO,
                    payload.AttachmentsList);

                await CreateOrUpdateIotDevicesCombo(saveCombo.Id, payload.DeviceComboList);

                return ResponseService<ComboDetailsResponseDTO>.OK(await GetComboDetailsById(saveCombo.Id));
            }
            catch (Exception e)
            {
                //return ResponseService<IotDeviceDetailsDTO>.BadRequest(e.Message);
                return ResponseService<ComboDetailsResponseDTO>.BadRequest("Cannot save the Combo Information. Please try again");
            }
        }
    }
}
