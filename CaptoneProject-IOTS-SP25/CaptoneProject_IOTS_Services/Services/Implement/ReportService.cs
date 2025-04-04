using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.DTO.RatingDTO;
using CaptoneProject_IOTS_BOs.DTO.ReportDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
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
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class ReportService : IReportService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
        private readonly IWalletService walletService;
        private readonly string REPORT_TITLE_PATTERN = "{UserEmail} send a report to {DeviceType} '{ProductName}'";
        public ReportService(UnitOfWork unitOfWork, IUserServices userServices, IWalletService walletService)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;
            this.walletService = walletService;
        }

        public async Task<ResponseDTO> ApproveOrRejectReport(int reportId, bool isApprove)
        {
            var report = unitOfWork.ReportRepository.Search(
                item => item.Id == item.Id && item.Status == (int)ReportStatusEnum.PENDING_TO_HANDLING)
                ?.Include(item => item.OrderItem)
                ?.ThenInclude(o => o.IotsDevice)
                ?.Include(item => item.OrderItem)
                ?.ThenInclude(o => o.Combo)
                ?.Include(item => item.OrderItem)
                ?.ThenInclude(o => o.Lab)
                ?.FirstOrDefault();

            if (report == null)
                return ResponseService<object>.NotFound("Report cannot be found. Please try again");

            var orderItem = report?.OrderItem;

            try
            {
                if (isApprove)
                {
                    report.Status = (int)ReportStatusEnum.COMPLETED;

                    var totalAmount = ((report?.OrderItem?.Price ?? 0) * (report?.OrderItem?.Quantity ?? 0) * ((decimal)ApplicationConst.FEE_PER_PRODUCT / 100))
                                                / 1000; //Convert to golds

                    var sellerId = orderItem?.SellerId;

                    if (sellerId == null)
                        return ResponseService<object>.NotFound("Seller cannot be found.");

                    Notifications notifications = new Notifications
                    {
                        EntityId = reportId,
                        EntityType = (int)EntityTypeEnum.REPORT,
                        Title = "Your Customer's Report was Handled Successfully by Admin",
                        Content = "Your Customer's Report was Handled Successfully by Admin",
                        ReceiverId = (int)sellerId
                    };

                    var updateWalletModel = new UpdateUserWalletRequestDTO
                    {
                        UserId = (int)sellerId,
                        Amount = totalAmount,
                    };

                    report = unitOfWork.ReportRepository.Update(report);

                    _ = walletService.UpdateUserWalletWithTransactionAsync([updateWalletModel]);

                    _ = unitOfWork.NotificationRepository.Create(notifications);

                    return ResponseService<object>.OK(new
                    {
                        ReportId = reportId
                    });
                }
                else //Rejected flow
                {
                    report.Status = (int)ReportStatusEnum.HANDLING_FAILED;

                    report = unitOfWork.ReportRepository.Update(report);

                    var sellerId = orderItem?.SellerId;

                    if (sellerId == null)
                        return ResponseService<object>.NotFound("Seller cannot be found.");

                    Notifications notifications = new Notifications
                    {
                        EntityId = reportId,
                        EntityType = (int)EntityTypeEnum.REPORT,
                        Title = "Your Customer's Report was Handled Failed",
                        Content = "Your Customer's Report was Handled Failed",
                        ReceiverId = (int)sellerId
                    };

                    _ = unitOfWork.NotificationRepository.Create(notifications);

                    return ResponseService<object>.OK(new
                    {
                        ReportId = reportId
                    });
                }
            } catch
            {
                return ResponseService<object>.BadRequest("You cannot approve or reject the Report. Please try again");
            }
            
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
    }
}
