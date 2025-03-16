using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.DTO.RatingDTO;
using CaptoneProject_IOTS_BOs.DTO.ReportDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class RatingService : IRatingService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
        private readonly string REPORT_TITLE_PATTERN = "{UserEmail} send a report to {DeviceType} '{ProductName}'";
        public RatingService(UnitOfWork unitOfWork, IUserServices userServices)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;
        }

        public Task<ResponseDTO> ApproveOrRejectReport(int reportId, bool isApprove)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO> GetFeedbackPagination(int productId, ProductTypeEnum productType, PaginationRequest request)
        {
            var pagination = unitOfWork.FeedbackRepository.GetPaginate(
                filter: item => (
                    (productType == ProductTypeEnum.IOT_DEVICE && item.OrderItem.IosDeviceId == productId) || 
                    (productType == ProductTypeEnum.COMBO && item.OrderItem.ComboId == productId) ||
                    (productType == ProductTypeEnum.LAB && item.OrderItem.LabId == productId)
                ),
                orderBy: ob => ob.OrderByDescending(item => item.CreatedDate),
                includeProperties: "OrderItem",
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            return ResponseService<object>.OK(pagination);
        }

        public ReportResponseDTO BuildReportResponseDTO(Report report, Store? store = null, User? createdBy = null, OrderItem? orderItem = null)
        {
            var res = GenericMapper<Report, ReportResponseDTO>.MapTo(report);

            res.ContactNumber = createdBy?.Phone;
            res.CreatedByEmail = createdBy?.Email;
            res.CreatedByFullname = createdBy?.Fullname;
            res.StoreId = store?.Id;
            res.StoreName = store?.Name;
            res.ProductType = orderItem?.ProductType;
            res.ProductId = (orderItem?.IosDeviceId != null) ? orderItem?.IosDeviceId : (orderItem?.LabId != null) ? orderItem?.LabId : orderItem?.ComboId;

            return res;
        }

        public async Task<ResponseDTO> GetReportPagination(PaginationRequest request)
        {
            var pagination = unitOfWork.ReportRepository.GetPaginate(
                //filter: ,
                orderBy: ob => ob.OrderByDescending(item => item.CreatedDate),
                includeProperties: "OrderItem,CreatedByNavigation",
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            var storeOwnerIds = pagination.Data?.Select(item => item?.OrderItem?.SellerId)?.Distinct()?.ToList();

            var storeList = unitOfWork.StoreRepository.Search(store => storeOwnerIds != null
                            && storeOwnerIds.Count(id => id != null && id == store.OwnerId) > 0);

            Func<Report, ReportResponseDTO> mapDto = item =>
            {
                var store = storeList?.FirstOrDefault(s => item.OrderItem != null && s.OwnerId == item.OrderItem.SellerId);

                return BuildReportResponseDTO(item, createdBy: item.CreatedByNavigation, store: store, orderItem: item.OrderItem);
            };

            var res = PaginationMapper<Report, ReportResponseDTO>.MapToByFunc(mapDto, pagination);

            return ResponseService<object>.OK(res);
        }

        public Feedback BuildFeedback(RatingRequestDTO source, int? loginUserId = null)
        {
            var model = GenericMapper<RatingRequestDTO, Feedback>.MapTo(source);

            model.CreatedDate = DateTime.Now;
            model.CreatedBy = loginUserId;

            return model;
        }

        private Report BuildReport(RatingRequestDTO source, 
            User? loginUser = null, 
            string? productName = null, 
            string? productType = null)
        {
            var model = GenericMapper<RatingRequestDTO, Report>.MapTo(source);

            model.CreatedDate = DateTime.Now;
            model.CreatedBy = loginUser?.Id;
            model.Title = REPORT_TITLE_PATTERN
                            .Replace("{UserEmail}", loginUser?.Email)
                            .Replace("{ProductName}", productName)
                            .Replace("{ProductType}", productType);

            return model;
        }

        public async Task<ResponseDTO> RatingProduct(List<RatingRequestDTO> request)
        {
            var loginUser = userServices.GetLoginUser();

            var orderItems = unitOfWork.OrderDetailRepository.Search(
                item => request.Count(item => item.OrderItemId == item.OrderItemId) > 0
                    && item.OrderItemStatus == (int)OrderItemStatusEnum.COMPLETED
            ).Include(item => item.IotsDevice)
            .Include(item => item.Combo)
            .Include(item => item.Lab);

            if (orderItems?.Count() != request.Count())
                return ResponseService<object>.BadRequest("You might already send your feedback. Please check again");

            try
            {
                var saveFeedbackList = request.Select(item => BuildFeedback(item, loginUser.Id));

                var saveReport = request.Where(item => item.Rating <= 2)
                                        .Select(item => {
                                            var orderItem = orderItems.SingleOrDefault(o => o.Id == item.OrderItemId);

                                            if (orderItem == null)
                                                throw new Exception();

                                            var productName = (orderItem.ProductType == (int)ProductTypeEnum.IOT_DEVICE) ?
                                                (orderItem?.IotsDevice?.Name) : (orderItem.ProductType == (int)ProductTypeEnum.COMBO) ?
                                                    (orderItem?.Combo?.Name) : orderItem?.Lab?.Title;

                                            var productType = (orderItem?.ProductType == (int)ProductTypeEnum.IOT_DEVICE) ?
                                                "Iot Device" : (orderItem?.ProductType == (int)ProductTypeEnum.COMBO) ?
                                                    "Combo" : "Playlist";

                                            return BuildReport(item, loginUser, productName, productType);
                                        });

                if (saveFeedbackList != null)
                    await unitOfWork.FeedbackRepository.CreateAsync(saveFeedbackList);

                if (saveReport != null)
                    await unitOfWork.ReportRepository.CreateAsync(saveReport);

                return ResponseService<object>.OK(null);

            } catch
            {
                return ResponseService<object>.BadRequest("You cannot send your feedback. Please try again");
            }
        }
    }
}
