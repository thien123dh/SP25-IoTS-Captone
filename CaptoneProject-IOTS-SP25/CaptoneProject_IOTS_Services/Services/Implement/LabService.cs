using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.NotificationDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderItemsDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Builder;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.OData.ModelBuilder;
using System.Collections.Generic;
using System.Linq.Expressions;
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class LabService : ILabService
    {
        private readonly IUserServices userServices;
        private readonly UnitOfWork unitOfWork;
        private readonly INotificationService notificationService;

        public LabService(IUserServices userServices, UnitOfWork unitOfWork, INotificationService notificationService)
        {
            this.userServices = userServices;
            this.unitOfWork = unitOfWork;
            this.notificationService = notificationService;
        }

        private string GetApplicationSerialNumber(int trainerId, string serialNumber)
        {
            return "LB{TrainerId}{SerialNumber}"
                .Replace("{TrainerId}", trainerId.ToString())
                .Replace("{SerialNumber}", serialNumber);
        }

        public async Task<GenericResponseDTO<LabDetailsInformationResponseDTO>> CreateOrUpdateLabDetailsInformation(int? labId, CreateUpdateLabInformationDTO request)
        {
            bool isTrainer = await userServices.CheckLoginUserRole(RoleEnum.TRAINER);
            var loginUserId = userServices.GetLoginUserId();

            if (!isTrainer || loginUserId == null)
                return ResponseService<LabDetailsInformationResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var lab = (labId == null) ? new Lab() : unitOfWork.LabRepository.GetById((int)labId);

            var createdBy = labId == null ? loginUserId : lab?.CreatedBy;
            var createdDate = lab.CreatedDate;
            var labStatus = lab.Status;
            var remark = lab.Remark;
            var rating = lab.Rating;

            if (labId > 0 && lab?.CreatedBy != loginUserId)
                return ResponseService<LabDetailsInformationResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            try
            {
                lab = GenericMapper<CreateUpdateLabInformationDTO, Lab>.MapTo(request);
                lab.Id = labId == null ? 0 : (int)labId;
                lab.CreatedBy = createdBy;
                lab.CreatedDate = createdDate;
                lab.UpdatedDate = DateTime.Now;
                lab.UpdatedBy = loginUserId;
                lab.Status = labStatus;
                lab.Remark = remark;
                lab.Rating = rating;
                lab.ApplicationSerialNumber = GetApplicationSerialNumber((int)loginUserId, lab.SerialNumber);

                var checkedExist = unitOfWork.LabRepository
                    .Search(item => item.ApplicationSerialNumber == lab.ApplicationSerialNumber && item.Id != labId)
                    .Any();

                if (checkedExist)
                    return ResponseService<LabDetailsInformationResponseDTO>.BadRequest("Duplicated Serial Number. Please try again");

                if (lab.Id > 0) //Update
                    lab = unitOfWork.LabRepository.Update(lab);
                else //Create
                    lab = unitOfWork.LabRepository.Create(lab);

                return ResponseService<LabDetailsInformationResponseDTO>.OK(await GetLabDetailsInformation(lab.Id));
            }
            catch
            {
                return ResponseService<LabDetailsInformationResponseDTO>.BadRequest("Cannot save lab information. Please try again");
            }
        }



        public async Task<ResponseDTO> GetComboLabsPagination(int comboId, PaginationRequest paginationRequest)
        {
            var res = await GetLabPagination(
                new LabFilterRequestDTO
                {
                    ComboId = comboId,
                    LabStatus = (int)ProductConst.LabStatusEnum.APPROVED
                },
                paginationRequest, 
                item => item.Status > 0
            );

            return res;
        }

        public async Task<LabDetailsInformationResponseDTO> GetLabDetailsInformation(int labId)
        {
            var lab = unitOfWork.LabRepository.GetById(labId);

            if (lab == null)
                throw new Exception(ExceptionMessage.LAB_NOTFOUND);

            try
            {
                var res = GenericMapper<Lab, LabDetailsInformationResponseDTO>.MapTo(lab);

                var ratingQuery = unitOfWork?.FeedbackRepository.Search(
                   item => labId == item.OrderItem.LabId
                ).Include(item => item.OrderItem);

                res.Rating = ((ratingQuery?.Sum(r => r.Rating) ?? 0) + ProductConst.DEFAULT_RATING) / ((ratingQuery?.Count() ?? 0) + 1);

                res.HasAbilityToViewPlaylist = await CheckPermissionToViewLabVideoList(res.Id);

                return res;
            }
            catch
            {
                throw new Exception("Error to get lab details information. Please try again");
            }

        }

        public async Task<ResponseDTO> GetLabPagination(LabFilterRequestDTO filterRequest,
                            PaginationRequest paginationRequest,
                            Expression<Func<Lab, bool>> additionalFunc)
        {
            var loginUserId = userServices.GetLoginUserId();
            var role = userServices.GetRole();

            Expression<Func<Lab, bool>> defaultFilter = item => (
                    (filterRequest.StoreId == null || filterRequest.StoreId == (int)item.ComboNavigation.StoreId)
                    &&
                    (filterRequest.UserId == null || filterRequest.UserId == item.CreatedBy)
                    &&
                    (filterRequest.ComboId == null || filterRequest.ComboId == item.ComboId)
                    &&
                    (filterRequest.LabStatus == null || (int)filterRequest.LabStatus == item.Status)
                    &&
                    (paginationRequest.SearchKeyword == null || item.Title.Contains(paginationRequest.SearchKeyword))
            );

            Expression<Func<Lab, bool>> finalFilter = defaultFilter;

            if (additionalFunc != null)
            {
                finalFilter = ExpressionBuilder.AndAlso<Lab>(finalFilter, additionalFunc);
            }

            var pagination = unitOfWork.LabRepository.GetPaginate(
                filter: finalFilter,
                orderBy: ob => ob.OrderByDescending(item => item.CreatedDate),
                includeProperties: "ComboNavigation,ComboNavigation.StoreNavigation",
                pageIndex: paginationRequest.PageIndex,
                pageSize: paginationRequest.PageSize
            );

            var labIds = pagination?.Data?.Select(item => item.Id).ToList();

            var labCarts = unitOfWork.CartRepository
                .Search(item => labIds != null && labIds.Contains(item.LabId ?? 0)).ToList();

            var successLabOrders = unitOfWork.OrderDetailRepository
                .Search(item => labIds != null && labIds.Contains(item.LabId ?? 0)
                        && (item.OrderItemStatus == (int)OrderItemStatusEnum.PENDING_TO_FEEDBACK || item.OrderItemStatus == (int)OrderItemStatusEnum.SUCCESS_ORDER)).ToList();

            var res = PaginationMapper<Lab, LabItemDTO>.MapTo(item => new LabItemDTO
            {
                ApplicationSerialNumber = item.ApplicationSerialNumber,
                ComboId = item.ComboId,
                ComboNavigationName = item?.ComboNavigation?.Name,
                CreatedBy = item?.CreatedBy,
                CreatedDate = item?.CreatedDate,
                Id = item?.Id ?? 0,
                ImageUrl = item?.ImageUrl,
                Status = item?.Status ?? 0,
                Price = item?.Price ?? 0,
                Rating = item?.Rating ?? 0,
                StoreId = item?.ComboNavigation?.StoreId ?? 0,
                StoreName = item?.ComboNavigation?.StoreNavigation?.Name,
                Summary = item?.Summary,
                Title = item?.Title,
                UpdatedBy = item?.UpdatedBy,
                UpdatedDate = item?.UpdatedDate,
                HasBeenAddToCartAlready = labCarts.Any(c => c.Id == (item?.Id ?? 0)),
                HasBeenBought = successLabOrders.Any(c => c.Id == (item?.Id ?? 0))
            }, pagination);

            return ResponseService<object>.OK(res);
        }

        public async Task<ResponseDTO> GetStoreManagementLabsPagination(int? comboId, PaginationRequest paginationRequest)
        {
            var loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null || !(await userServices.CheckLoginUserRole(RoleEnum.STORE)))
                return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var store = unitOfWork.StoreRepository.GetByUserId((int)loginUserId);

            if (store == null)
                return ResponseService<object>.NotFound(ExceptionMessage.STORE_NOTFOUND);

            Expression<Func<Lab, bool>> storeFilter = item => item.ComboNavigation.CreatedBy == loginUserId &&
                                                  item.Status != (int)LabStatusEnum.DRAFT;

            var res = await GetLabPagination(new LabFilterRequestDTO
            {
                StoreId = store?.Id,
                ComboId = comboId
            }, paginationRequest, storeFilter);

            return res;
        }

        public async Task<ResponseDTO> GetManagerAdminManagementLabsPagination(PaginationRequest paginationRequest)
        {
            var loginUserId = userServices.GetLoginUserId();
            var role = userServices.GetRole();

            if (loginUserId == null || (role != (int)RoleEnum.ADMIN && role != (int)RoleEnum.MANAGER))
                return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            Expression<Func<Lab, bool>> storeFilter = item => true;

            var res = await GetLabPagination(new LabFilterRequestDTO
            {
            }, paginationRequest, storeFilter);

            return res;
        }

        public async Task<ResponseDTO> GetTrainerManagementLabsPagination(LabFilterRequestDTO filterRequest, PaginationRequest paginationRequest)
        {
            int? loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            Expression<Func<Lab, bool>> trainerFilter = item => item.CreatedBy == loginUserId;

            var res = await GetLabPagination(
                filterRequest,
                paginationRequest,
                trainerFilter
            );

            return res;
        }

        public async Task<bool> CheckPermissionToViewLabVideoList(int labId)
        {
            var loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return false;

            var loginUserRoles = (await userServices.GetLoginUserRoles());

            bool isStore = loginUserRoles?.Count(i => i.Id == (int)RoleEnum.STORE) > 0;
            bool isTrainer = loginUserRoles?.Count(i => i.Id == (int)RoleEnum.TRAINER) > 0;
            bool isCustomer = loginUserRoles?.Count(i => i.Id == (int)RoleEnum.CUSTOMER) > 0;

            var lab = unitOfWork.LabRepository.GetById(labId);

            if (isStore) //IF ROLE IS STORE
            {
                if (lab == null)
                    return false;

                return lab.ComboNavigation?.CreatedBy == loginUserId;
            }
            else if (isTrainer) //Is Trainer
            {
                return lab?.CreatedBy == loginUserId;
            }
            else if (isCustomer) //Is role customer
            {
                var doCustomerBuyLab = unitOfWork.OrderDetailRepository.Search(
                    item => item.LabId == labId && item.OrderBy == (int)loginUserId &&
                    (item.OrderItemStatus == (int)OrderItemStatusEnum.SUCCESS_ORDER
                    || item.OrderItemStatus == (int)OrderItemStatusEnum.PENDING_TO_FEEDBACK)
                ).Any();

                return doCustomerBuyLab;
            }

            return false;
        }

        public async Task<GenericResponseDTO<List<LabVideoResponseDTO>>> GetLabVideoList(int labId)
        {
            var checkPermission = await CheckPermissionToViewLabVideoList(labId);

            if (!checkPermission)
                return ResponseService<List<LabVideoResponseDTO>>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var labVideoList = unitOfWork.LabAttachmentRepository.GetByLabId(labId);

            if (labVideoList == null)
                return ResponseService<List<LabVideoResponseDTO>>.NotFound("No Video Found");

            return ResponseService<List<LabVideoResponseDTO>>.OK(labVideoList?.Select(item =>
                GenericMapper<LabAttachment, LabVideoResponseDTO>.MapTo(item)
            )?.ToList());
        }

        public async Task<GenericResponseDTO<List<LabVideoResponseDTO>>> CreateOrUpdateLabVideoList(int labId, List<CreateUpdateLabVideo> requestList)
        {
            var lab = unitOfWork.LabRepository.GetById(labId);

            if (lab == null)
                ResponseService<LabDetailsInformationResponseDTO>.NotFound(ExceptionMessage.LAB_NOTFOUND);

            var loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null || lab.CreatedBy == loginUserId)
                ResponseService<LabDetailsInformationResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var labVideoList = unitOfWork.LabAttachmentRepository.GetByLabId(labId);

            var removeList = labVideoList.Where(item => !requestList.Where(i => i.Id == item.Id).Any()).ToList();

            try
            {
                await unitOfWork.LabAttachmentRepository.RemoveAsync(removeList);

                int count = 0;

                var saveVideoList = requestList.Select(
                    item => new LabAttachment
                    {
                        Id = item.Id,
                        LabId = item.LabId,
                        CreatedDate = DateTime.Now,
                        Description = item.Description,
                        Title = item.Title,
                        UpdatedDate = DateTime.Now,
                        VideoUrl = item.VideoUrl,
                        OrderIndex = ++count
                    }
                ).ToList();

                var createList = saveVideoList?.Where(i => i.Id <= 0);
                var updateList = saveVideoList?.Where(i => i.Id > 0);

                if (createList != null) //Create
                    await unitOfWork.LabAttachmentRepository.CreateAsync(createList);

                if (updateList != null)
                    await unitOfWork.LabAttachmentRepository.UpdateAsync(updateList);

                return await GetLabVideoList(labId);
            }
            catch (Exception ex)
            {
                return ResponseService<List<LabVideoResponseDTO>>.BadRequest("Cannot create or update video list. Please try again");
            }
        }

        public async Task<GenericResponseDTO<LabDetailsInformationResponseDTO>> ApproveOrRejectLab(int labId, bool isApprove, RemarkDTO? payload = null)
        {
            int? loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<LabDetailsInformationResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var store = unitOfWork.StoreRepository.GetByUserId((int)loginUserId);

            if (store == null)
                return ResponseService<LabDetailsInformationResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var lab = unitOfWork.LabRepository.GetById(labId);

            if (lab == null)
                return ResponseService<LabDetailsInformationResponseDTO>.NotFound(ExceptionMessage.LAB_NOTFOUND);

            if (lab?.ComboNavigation?.CreatedBy != loginUserId)
                return ResponseService<LabDetailsInformationResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            if (lab?.Status != (int)LabStatusEnum.PENDING_TO_APPROVE)
                return ResponseService<LabDetailsInformationResponseDTO>.BadRequest("Playlist has been already approved or rejected. Please try again");

            try
            {
                lab.Status = (isApprove) ? (int)LabStatusEnum.APPROVED : (int)LabStatusEnum.REJECTED;

                lab.Remark = (!isApprove) ? payload?.Remark : lab.Remark;

                lab = unitOfWork.LabRepository.Update(lab);

                var notification = new NotificationRequestDTO();

                //SENDING NOTIFICATION
                if (isApprove)
                {
                    notification = new NotificationRequestDTO
                    {
                        EntityId = lab.Id,
                        EntityType = (int)EntityTypeEnum.LAB,
                        Title = "Congratulation! Your Playlist '{labName}' was approved by Store '{storeName}'"
                                    .Replace("{labName}", lab.Title)
                                    .Replace("{storeName}", store.Name),
                        Content = "Congratulation! Your Playlist '{labName}' was approved by Store '{storeName}'"
                                    .Replace("{labName}", lab.Title)
                                    .Replace("{storeName}", store.Name),
                        ReceiverId = (int)lab.CreatedBy
                    };
                }
                else
                {
                    notification = new NotificationRequestDTO
                    {
                        EntityId = lab.Id,
                        EntityType = (int)EntityTypeEnum.LAB,
                        Content = "Your Playlist '{labName}' was rejected by Store '{storeName}'. Please check the remarks"
                                    .Replace("{labName}", lab.Title)
                                    .Replace("{storeName}", store.Name),
                        Title = "Your Playlist '{labName}' was rejected by Store '{storeName}'. Please check the remarks"
                                    .Replace("{labName}", lab.Title)
                                    .Replace("{storeName}", store.Name),
                        ReceiverId = (int)lab.CreatedBy
                    };
                }

                _ = notificationService.CreateUserNotification([notification]);
            }
            catch
            {
                return ResponseService<LabDetailsInformationResponseDTO>.BadRequest("Cannot approve or reject the playlist. Please try again");

            }

            var res = await GetLabDetailsInformation(lab.Id);

            return ResponseService<LabDetailsInformationResponseDTO>.OK(res);
        }

        public async Task<GenericResponseDTO<LabDetailsInformationResponseDTO>> SubmitLabRequest(int labId)
        {
            var lab = unitOfWork.LabRepository.GetById(labId);
            int? loginUserId = userServices.GetLoginUserId();

            if (lab == null)
                return ResponseService<LabDetailsInformationResponseDTO>.NotFound(ExceptionMessage.LAB_NOTFOUND);

            if (lab.CreatedBy != loginUserId)
                return ResponseService<LabDetailsInformationResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            if (lab.Status != (int)LabStatusEnum.REJECTED && lab.Status != (int)LabStatusEnum.DRAFT)
                return ResponseService<LabDetailsInformationResponseDTO>.BadRequest("Your request has already sent or approved. Please check again");

            lab.Status = (int)LabStatusEnum.PENDING_TO_APPROVE;

            try
            {
                lab = unitOfWork.LabRepository.Update(lab);
            }
            catch
            {
                ResponseService<LabDetailsInformationResponseDTO>.BadRequest("Cannot save the Lab. Please try again");
            }

            var res = await GetLabDetailsInformation(lab.Id);

            var notification = new NotificationRequestDTO
            {
                EntityId = labId,
                EntityType = (int)EntityTypeEnum.LAB,
                Content = "The playlist '{labName}' was submitted for you to approve. Please check go to the details and check"
                                    .Replace("{labName}", lab.Title),
                ReceiverId = (int)lab?.ComboNavigation?.CreatedBy,
                Title = "The playlist '{labName}' was submitted for you to approve. Please check go to the details and check"
                                    .Replace("{labName}", lab.Title)
            };

            _ = notificationService.CreateUserNotification([notification]);

            return ResponseService<LabDetailsInformationResponseDTO>.OK(res);
        }

        public async Task<ResponseDTO> GetCustomerManagementLabsPagination(PaginationRequest paginationRequest)
        {
            int? loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var orderSuccessLabOrder = unitOfWork.OrderDetailRepository.Search(
                item => item.LabId != null && item.OrderBy == loginUserId && (item.OrderItemStatus == (int)OrderItemStatusEnum.PENDING_TO_FEEDBACK ||
                item.OrderItemStatus == (int)OrderItemStatusEnum.SUCCESS_ORDER)
            )?.Select(item => item.LabId)?.ToList();

            Expression<Func<Lab, bool>> customerPermissionFilter = item => item.CreatedBy == item.CreatedBy
                                            && (orderSuccessLabOrder != null) && orderSuccessLabOrder.Contains(item.Id);

            var res = await GetLabPagination(
                new LabFilterRequestDTO(),
                paginationRequest,
                customerPermissionFilter
            );

            return res;
        }

        public async Task<ResponseDTO> ActiveOrDeactiveLab(int labId, bool isDeactive = false)
        {
            var loginUserId = (int)userServices.GetLoginUserId();
            var role = userServices.GetRole();

            var lab = unitOfWork.LabRepository.GetById(labId);

            if (lab == null)
                return ResponseService<object>.NotFound("Lab cannot be found. Please try again");

            var notiMessage = "";

            if (isDeactive)
            {
                lab.Status = 0;
                lab.UpdatedDate = DateTime.Now;
                lab.UpdatedBy = loginUserId;

                if (loginUserId != lab.CreatedBy)
                {
                    notiMessage = $"Your lab '{lab.Title}' has been deactivated by Manager. Please check the reason";
                }
            } else
            {
                lab.Status = 1;
                lab.UpdatedDate = DateTime.Now;
                lab.UpdatedBy = loginUserId;

                if (loginUserId != lab.CreatedBy)
                {
                    notiMessage = $"Your lab '{lab.Title}' has been activated by Manager";
                }
            }

            lab = unitOfWork.LabRepository.Update(lab);

            if (notiMessage != "")
            {
                var noti = new Notifications
                {
                    Content = notiMessage,
                    Title = notiMessage,
                    EntityId = lab.Id,
                    EntityType = (int)EntityTypeEnum.LAB,
                    ReceiverId = (int)lab.CreatedBy
                };

                _ = unitOfWork.NotificationRepository.Create(noti);
            }

            return ResponseService<object>.OK(lab);
        }
    }
}
