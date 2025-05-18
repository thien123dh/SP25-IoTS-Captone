using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.FeedbackDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class FeedbackService : IFeedbackService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
        private readonly int MIN_REPORT_RATING = 2;
        private readonly IWalletService walletService;
        private readonly int APPLICATION_FEE;
        public FeedbackService(UnitOfWork unitOfWork, 
            IUserServices userServices, 
            IWalletService walletService, 
            IGeneralSettingsService generalSettingsService)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;
            this.walletService = walletService;

            APPLICATION_FEE = unitOfWork?.GeneralSettingsRepository?.GetAll()?.FirstOrDefault()?.ApplicationFeePercent ?? 10;
        }

        public ResponseDTO CheckOrderFeedbackValidation(
            List<OrderItem> orderItems,
            StoreOrderFeedbackRequestDTO request)
        {
            var notContains = orderItems.Any(item => !request.FeedbackList.Any(f => f.OrderItemId == item.Id));

            if (notContains)
                return ResponseService<object>.BadRequest("Some products you have not feedback. Please check again");

            return ResponseService<object>.OK(null);
        }

        public Feedback BuildToFeedback(FeedbackItemRequestDTO source)
        {
            var loginUserId = userServices.GetLoginUserId();

            return new Feedback
            {
                Content = source.Comment,
                CreatedBy = loginUserId,
                CreatedDate = DateTime.Now,
                OrderItemId = source.OrderItemId,
                Rating = source.Rating
            };
        }

        public async Task<ResponseDTO> CreateOrderFeedback(StoreOrderFeedbackRequestDTO request)
        {
            var loginUserId = userServices.GetLoginUserId();

            var loginUser = userServices.GetLoginUser();

            var order = unitOfWork.OrderRepository.GetById(request.OrderId);

            if (order.CreatedBy != loginUserId)
                return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var store = unitOfWork.StoreRepository.GetById(request.SellerId);

            var sellerOrderList = unitOfWork.OrderDetailRepository.Search(
                item => item.OrderId == request.OrderId 
                        && item.SellerId == request.SellerId
                        && item.OrderItemStatus == (int)OrderItemStatusEnum.PENDING_TO_FEEDBACK
            ).Include(item => item.Lab)
            .ThenInclude(lab => lab.CreatedByNavigation).ToList();

            var checkValidation = CheckOrderFeedbackValidation(sellerOrderList, request);

            if (!checkValidation.IsSuccess)
                return checkValidation;
            
            var sellerWalletUpdateRequest = new List<UpdateUserWalletRequestDTO>();

            Dictionary<int, decimal> sellerWalletUpdateRequestMap = new Dictionary<int, decimal>();

            try
            {
                var feedbackList = request.FeedbackList.Select(
                    f => BuildToFeedback(f)
                );

                List<Report> reports = new List<Report>();

                var updatedOrderItems = sellerOrderList.Select(
                    item =>
                    {
                        var feedback = feedbackList.FirstOrDefault(f => f.OrderItemId == item.Id);

                        if (feedback?.Rating <= MIN_REPORT_RATING) //Create report
                        {
                            item.OrderItemStatus = (int)OrderItemStatusEnum.BAD_FEEDBACK;

                            var title = (item.ProductType == (int)ProductTypeEnum.LAB) ?
                                        "{Email} send a report to playlist of a trainer '{TrainerEmail}'"
                                        .Replace("{Email}", loginUser.Email)
                                        .Replace("{TrainerEmail}", item?.Lab?.CreatedByNavigation?.Email)
                                        : 
                                        "{Email} send a report to product of store '{StoreName}'"
                                        .Replace("{Email}", loginUser.Email)
                                        .Replace("{StoreName}", store?.Name);

                            reports.Add(
                                new Report
                                {
                                    Content = feedback.Content,
                                    CreatedBy = loginUserId,
                                    CreatedDate = DateTime.Now,
                                    OrderItemId = feedback.OrderItemId,
                                    Title = title,
                                    Status = (int)ReportStatusEnum.PENDING_TO_HANDLING
                                }
                            );
                        }
                        else
                        {
                            if (!sellerWalletUpdateRequestMap.ContainsKey(item.SellerId))
                                sellerWalletUpdateRequestMap.Add(item.SellerId, 0);

                            var amount = sellerWalletUpdateRequestMap.GetValueOrDefault(item.SellerId);

                            sellerWalletUpdateRequestMap[item.SellerId] = amount + (item.Price) * (item.Quantity);

                            item.OrderItemStatus = (int)OrderItemStatusEnum.SUCCESS_ORDER;
                        }

                        return item;
                    }
                )?.ToList();

                var updateWalletRequest = new List<UpdateUserWalletRequestDTO>();

                decimal appReceived = 0;

                foreach (var userId in sellerWalletUpdateRequestMap.Keys)
                {
                    var amount = sellerWalletUpdateRequestMap.GetValueOrDefault(userId);

                    appReceived += (amount * ((decimal)APPLICATION_FEE) / 100 ) / 1000;

                    updateWalletRequest.Add(
                        new UpdateUserWalletRequestDTO
                        {
                            UserId = userId,
                            Amount = (amount * (((decimal)100 - APPLICATION_FEE) / 100)) / 1000
                        }
                    );
                }

                Transaction appTrans = new Transaction
                {
                    Amount = appReceived,
                    CreatedDate = DateTime.Now,
                    CurrentBallance = 0,
                    Description = $"You have received {appReceived} gold for Success Order {order.ApplicationSerialNumber} / Seller: {request.SellerId}",
                    Status = "Success",
                    TransactionType = $"Order {order.ApplicationSerialNumber}",
                    UserId = AdminConst.ADMIN_ID,
                    IsApplication = 1
                };

                if (updatedOrderItems != null)
                    await unitOfWork.OrderDetailRepository.UpdateAsync(updatedOrderItems);

                _ = walletService.UpdateUserWalletOrderTransactionAsync(updateWalletRequest, order.ApplicationSerialNumber).ConfigureAwait(false);

                _ = unitOfWork.FeedbackRepository.CreateAsync(feedbackList);

                if (appReceived > 0)
                    _ = unitOfWork.TransactionRepository.Create(appTrans);
                
                if (reports != null)
                {
                    _ = unitOfWork.ReportRepository.CreateAsync(reports);
                }
                    

                return ResponseService<object>.OK(
                    new
                    {
                        SellerId = request.SellerId,
                        SellerRole = request.SellerRole,
                        OrderId = request.OrderId
                    }
                );
            }
            catch (Exception ex)
            {
                return ResponseService<object>.BadRequest("Feedback Error. Please try again");
            }
        }

        public async Task<ResponseDTO> GetFeedbackPagination(ProductRequestDTO productRequest, 
            PaginationRequest paginationRequest)
        {
            var productType = productRequest.ProductType;
            var productId = productRequest.ProductId;

            Expression<Func<Feedback, bool>> orderFilter = item =>
                (productType == ProductTypeEnum.LAB && item.OrderItem.LabId == productId) ||
                (productType == ProductTypeEnum.COMBO && item.OrderItem.ComboId == productId) ||
                (productType == ProductTypeEnum.IOT_DEVICE && item.OrderItem.IosDeviceId == productId);

            var res = unitOfWork.FeedbackRepository.GetPaginate(
                filter: orderFilter,
                orderBy: ob => ob.OrderByDescending(f => f.CreatedDate),
                includeProperties: "OrderItem",
                pageIndex: paginationRequest.PageIndex,
                pageSize: paginationRequest.PageSize
            );

            return ResponseService<object>.OK(res);
        }
    }
}
