using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.RatingDTO;
using CaptoneProject_IOTS_BOs.DTO.ReportDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class ReportService : IReportService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
        private readonly IWalletService walletService;
        private readonly string REPORT_TITLE_PATTERN = "{UserEmail} send a report to {DeviceType} '{ProductName}'";
        private readonly int APPLICATION_FEE;
        public ReportService(UnitOfWork unitOfWork, IUserServices userServices, IWalletService walletService)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;
            this.walletService = walletService;

            APPLICATION_FEE = unitOfWork?.GeneralSettingsRepository?.Search(item => true)?.FirstOrDefault()?.ApplicationFeePercent ?? 0;
        }

        public async Task<ResponseDTO> ApproveOrRejectReport(int reportId, bool isApprove)
        {
            var report = unitOfWork.ReportRepository.Search(
                item => item.Id == reportId && item.Status == (int)ReportStatusEnum.PENDING_TO_HANDLING)
                ?.Include(item => item.OrderItem)
                ?.ThenInclude(o => o.IotsDevice)
                ?.Include(item => item.OrderItem)
                ?.ThenInclude(o => o.Combo)
                ?.Include(item => item.OrderItem)
                ?.ThenInclude(o => o.Lab)
                ?.Include(o => o.OrderItem)
                ?.ThenInclude(o => o.Order)
                ?.FirstOrDefault();

            if (report == null)
                return ResponseService<object>.NotFound("Report cannot be found. Please try again");

            var orderItem = report?.OrderItem;

            try
            {
                if (isApprove)
                {
                    report.Status = (int)ReportStatusEnum.COMPLETED;

                    var totalAmount = ((report?.OrderItem?.Price ?? 0) * (report?.OrderItem?.Quantity ?? 0));

                    totalAmount = (totalAmount * (((decimal)100 - APPLICATION_FEE) / 100)) / 1000;
                    var appAmount = (totalAmount * ((APPLICATION_FEE) / 100)) / 1000;

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

                    //orderItem.OrderItemStatus = (int)OrderItemStatusEnum.SUCCESS_ORDER;

                    report = unitOfWork.ReportRepository.Update(report);

                    //unitOfWork.OrderDetailRepository.Update(orderItem);

                    _ = walletService.UpdateUserWalletOrderTransactionAsync([updateWalletModel]);

                    Transaction appTrans = new Transaction
                    {
                        Amount = appAmount,
                        CreatedDate = DateTime.Now,
                        CurrentBallance = 0,
                        Description = $"You have received {appAmount} gold for Order {orderItem.Order.ApplicationSerialNumber} / Seller {orderItem.SellerId}",
                        Status = "Success",
                        TransactionType = $"Order {orderItem.Order.ApplicationSerialNumber}",
                        UserId = AdminConst.ADMIN_ID,
                        IsApplication = 1
                    };

                    _ = unitOfWork.NotificationRepository.Create(notifications);
                    
                    _ = unitOfWork.TransactionRepository.Create(appTrans);

                    return ResponseService<object>.OK(new
                    {
                        ReportId = reportId
                    });
                }
                else //Rejected flow
                {
                    report.Status = (int)ReportStatusEnum.REFUNDED;

                    report = unitOfWork.ReportRepository.Update(report);

                    var sellerId = orderItem?.SellerId;

                    if (sellerId == null)
                        return ResponseService<object>.NotFound("Seller cannot be found");

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
            }
            catch
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
            res.ProductName = orderItem?.IotsDevice?.Name ?? orderItem?.Combo?.Name ?? orderItem?.Lab?.Title ?? "";
            res.OrderCode = orderItem?.Order?.ApplicationSerialNumber;

            return res;
        }

        public async Task<ResponseDTO> GetReportPagination(int? filterStatus, PaginationRequest request)
        {
            var loginUserId = userServices.GetLoginUserId();
            var role = userServices.GetRole();

            var adminOrStaffRoles = new List<int> { (int)RoleEnum.ADMIN, (int)RoleEnum.STAFF };

            Expression<Func<Report, bool>> func = item => false;

            if (adminOrStaffRoles.Contains((int)role))
            {
                func = item => (filterStatus == null || item.Status == filterStatus);
            }
            else if (role == (int)RoleEnum.CUSTOMER)
            {
                func = item => (filterStatus == null || item.Status == filterStatus) && item.CreatedBy == loginUserId;
            }
            else
            {
                return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);
            }

            var pagination = unitOfWork.ReportRepository.GetPaginate(
                filter: func,
                orderBy: ob => ob.OrderByDescending(item => item.CreatedDate),
                includeProperties: "OrderItem,OrderItem.Order,OrderItem.IotsDevice,OrderItem.Combo,OrderItem.Lab,CreatedByNavigation",
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
