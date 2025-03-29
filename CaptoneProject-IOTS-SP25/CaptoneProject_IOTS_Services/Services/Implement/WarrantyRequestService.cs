using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.NotificationDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.DTO.WarrantyRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class WarrantyRequestService : IWarrantyRequestService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
        private readonly IStoreService storeService;
        private readonly INotificationService notificationService;

        public WarrantyRequestService(UnitOfWork unitOfWork,
            IUserServices userServices,
            IStoreService storeService,
            INotificationService notificationService)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;
            this.storeService = storeService;
            this.notificationService = notificationService;
        }

        public WarrantyRequest BuildToWarrantyRequest(WarrantyRequest target, WarrantyRequestRequestDTO request)
        {
            target.OrderItemId = request.OrderItemId;
            target.VideoUrl = request.VideoUrl;
            target.ContactNumber = request.ContactNumber;
            target.IdentifySerialNumber = request.IdentifySerialNumber;
            target.Description = request.Description;

            return target;
        }

        public async Task<GenericResponseDTO<WarrantyRequestResponseDTO>> CreateCustomerWarrantyRequest(WarrantyRequestRequestDTO request)
        {
            var loginUserId = userServices.GetLoginUserId();

            var role = userServices.GetRole();

            if (role != (int)RoleEnum.CUSTOMER)
                return ResponseService<WarrantyRequestResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var loginUser = userServices.GetLoginUser();

            try
            {
                var orderItem = unitOfWork.OrderDetailRepository.GetById(request.OrderItemId);

                if (orderItem == null)
                {
                    return ResponseService<WarrantyRequestResponseDTO>.BadRequest("Order cannot be found. Please try again");
                }
                else if (orderItem.OrderItemStatus != (int)OrderItemStatusEnum.SUCCESS_ORDER && orderItem.OrderItemStatus != (int)OrderItemStatusEnum.CANCELLED)
                {
                    return ResponseService<WarrantyRequestResponseDTO>.BadRequest("Please feedback this order before create warranty request. Please try again");
                }
                else if (orderItem.WarrantyEndDate < DateTime.Now)
                {
                    return ResponseService<WarrantyRequestResponseDTO>.BadRequest("Warranty Date of this product was Expired");
                }

                var warrantyRequest = new WarrantyRequest
                {
                    CreatedBy = (int)loginUserId
                };

                warrantyRequest = BuildToWarrantyRequest(warrantyRequest, request);

                warrantyRequest = unitOfWork.WarrantyRequestRepository.Create(warrantyRequest);

                var notification = new NotificationRequestDTO
                {
                    Content = $"Your receiver a warranty request by {loginUser.Email}",
                    Title = $"Your receiver a warranty request by {loginUser.Email}",
                    EntityId = warrantyRequest.Id,
                    EntityType = (int)EntityTypeEnum.WARRANTY_REQUEST,
                    ReceiverId = orderItem.SellerId
                };

                _ = notificationService.CreateUserNotification([notification]);

                return await GetWarrantyRequestById(warrantyRequest.Id);
            }
            catch (Exception ex)
            {
                return ResponseService<WarrantyRequestResponseDTO>.BadRequest("You cannot create warranty request. Please try again");

            }
        }

        public async Task<WarrantyRequestResponseDTO> BuildToWarrantyRequestResponseDTO(WarrantyRequest source, Store? store = null)
        {
            var res = GenericMapper<WarrantyRequest, WarrantyRequestResponseDTO>.MapTo(source);

            res.OrderId = source.OrderItem.OrderId;

            res.ProductType = source.OrderItem.ProductType;

            var orderItem = source.OrderItem;

            res.ProductId = orderItem?.IosDeviceId ?? orderItem?.ComboId ?? orderItem?.LabId ?? 0;

            res.ProductName = orderItem?.IotsDevice?.Name ?? orderItem?.Combo?.Name ?? orderItem?.Lab?.Title ?? "";

            if (store != null)
                res.StoreInfo = await storeService.BuildToStoreDetailsResponseDTO(store);

            return res;
        }

        public async Task<GenericResponseDTO<WarrantyRequestResponseDTO>> GetWarrantyRequestById(int id)
        {
            var loginUserId = userServices.GetLoginUserId();

            var warranty = unitOfWork.WarrantyRequestRepository?.Search(item => item.Id == id)?
                    .Include(item => item.OrderItem)
                    .ThenInclude(o => o.Lab)
                    .Include(item => item.OrderItem)
                    .ThenInclude(o => o.IotsDevice)
                    .Include(item => item.OrderItem)
                    .ThenInclude(o => o.Combo)
                    .SingleOrDefault(item => item.Id == id);

            if (warranty == null)
                return ResponseService<WarrantyRequestResponseDTO>.NotFound("Your warranty request cannot be found. Please try again");

            if (warranty.CreatedBy != loginUserId && warranty.OrderItem.SellerId != loginUserId)
                return ResponseService<WarrantyRequestResponseDTO>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var store = unitOfWork.StoreRepository.Search(s => s.OwnerId == warranty.OrderItem.SellerId).FirstOrDefault();

            var response = await BuildToWarrantyRequestResponseDTO(warranty, store);

            return ResponseService<WarrantyRequestResponseDTO>.OK(response);
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<WarrantyRequestResponseDTO>>> GetWarrantyRequestPagination(WarrantyRequestStatusEnum? statusFilter, PaginationRequest request)
        {
            var loginUserId = (int)userServices.GetLoginUserId();
            var role = (int)userServices.GetRole();

            Expression<Func<WarrantyRequest, bool>> func = item => false;

            if (role == (int)RoleEnum.CUSTOMER)
            {
                func = item => item.CreatedBy == loginUserId && (statusFilter == null || item.Status == (int)statusFilter);
            }
            else if (role == (int)RoleEnum.STORE)
            {
                func = item => item.OrderItem.SellerId == loginUserId && (statusFilter == null || item.Status == (int)statusFilter);
            }

            var pagination = unitOfWork.WarrantyRequestRepository.GetPaginate(
                filter: func,
                orderBy: ob => ob.OrderByDescending(item => item.CreatedDate),
                includeProperties: "OrderItem,OrderItem.IotsDevice,OrderItem.Lab,OrderItem.Combo",
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            var sellerIds = pagination?.Data?.Select(i => i.OrderItem.SellerId).ToList();

            var storeList = unitOfWork.StoreRepository.Search(item => !sellerIds.IsNullOrEmpty() && sellerIds.Any(id => id == item.OwnerId)).ToList();

            var response = new PaginationResponseDTO<WarrantyRequestResponseDTO>
            {
                PageIndex = pagination.PageIndex,
                PageSize = pagination.PageSize,
                TotalCount = pagination.TotalCount
            };

            response.Data = (await Task.WhenAll(
                pagination.Data.Select(async item =>
                {
                    var store = storeList.FirstOrDefault(s => s.OwnerId == item.OrderItem.SellerId);

                    return await BuildToWarrantyRequestResponseDTO(item, store);
                }).ToList()
            ));

            return ResponseService<PaginationResponseDTO<WarrantyRequestResponseDTO>>.OK(response);
        }

        public async Task<GenericResponseDTO<WarrantyRequestResponseDTO>> StoreApproveOrRejectWarrantyRequest(int id, bool isApprove, RemarkDTO? remarks = null)
        {
            var warranty = unitOfWork.WarrantyRequestRepository.GetById(id);
            NotificationRequestDTO notification = new NotificationRequestDTO();

            if (warranty.Status != (int)WarrantyRequestStatusEnum.PENDING_TO_APPROVE)
                return ResponseService<WarrantyRequestResponseDTO>.BadRequest("This warranty request already handled. Please check again");

            if (isApprove)
            {
                warranty.Status = (int)WarrantyRequestStatusEnum.APPROVED;
                warranty.ActionDate = DateTime.Now;

                notification = new NotificationRequestDTO
                {
                    Content = $"Your warranty request has been approved. Please check and contact with store",
                    Title = $"Your warranty request has been approved. Please check and contact with store",
                    EntityId = warranty.Id,
                    EntityType = (int)EntityTypeEnum.WARRANTY_REQUEST,
                    ReceiverId = warranty.CreatedBy
                };
            }
            else
            {
                warranty.Remarks = remarks?.Remark;
                warranty.Status = (int)WarrantyRequestStatusEnum.REJECTED;
                warranty.ActionDate = DateTime.Now;

                notification = new NotificationRequestDTO
                {
                    Content = $"Your warranty request has been rejected. Please check remarks",
                    Title = $"Your warranty request has been rejected. Please check remarks",
                    EntityId = warranty.Id,
                    EntityType = (int)EntityTypeEnum.WARRANTY_REQUEST,
                    ReceiverId = warranty.CreatedBy
                };
            }

            warranty = unitOfWork.WarrantyRequestRepository.Update(warranty);

            _ = notificationService.CreateUserNotification([notification]);

            return await GetWarrantyRequestById(warranty.Id);
        }

        public async Task<GenericResponseDTO<WarrantyRequestResponseDTO>> ConfirmSuccess(int id)
        {
            var loginUser = userServices.GetLoginUser();

            var warranty = unitOfWork.WarrantyRequestRepository
                .Search(item => item.Id == id)
                .Include(item => item.OrderItem)
                .FirstOrDefault();

            if (warranty == null)
                return ResponseService<WarrantyRequestResponseDTO>.NotFound("Warrant request cannot be found. Please try again");

            NotificationRequestDTO notification = new NotificationRequestDTO();

            if (warranty.Status != (int)WarrantyRequestStatusEnum.APPROVED)
                return ResponseService<WarrantyRequestResponseDTO>.BadRequest("This warranty request already handled. Please check again");

            warranty.Status = (int)WarrantyRequestStatusEnum.SUCCESS;
            warranty.ActionDate = DateTime.Now;

            notification = new NotificationRequestDTO
            {
                Content = $"Customer {loginUser.Email} already confirmed your warranty service",
                Title = $"Customer {loginUser.Email} already confirmed your warranty service",
                EntityId = warranty.Id,
                EntityType = (int)EntityTypeEnum.WARRANTY_REQUEST,
                ReceiverId = warranty.OrderItem.SellerId
            };

            warranty = unitOfWork.WarrantyRequestRepository.Update(warranty);

            _ = notificationService.CreateUserNotification([notification]);

            return await GetWarrantyRequestById(warranty.Id);
        }
    }
}
