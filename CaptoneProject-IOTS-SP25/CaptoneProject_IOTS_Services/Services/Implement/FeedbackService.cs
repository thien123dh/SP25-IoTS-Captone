using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.FeedbackDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public FeedbackService(UnitOfWork unitOfWork, IUserServices userServices)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;
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

            var store = unitOfWork.StoreRepository.GetByUserId(request.SellerId);

            if (store == null)
                return ResponseService<object>.NotFound(ExceptionMessage.STORE_NOTFOUND);

            var storeOrderList = unitOfWork.OrderDetailRepository.Search(
                item => item.OrderId == request.OrderId 
                        && item.SellerId == store.OwnerId 
                        && item.OrderItemStatus == (int)OrderItemStatusEnum.PENDING_TO_FEEDBACK
            ).Include(item => item.Lab)
            .ThenInclude(lab => lab.CreatedByNavigation).ToList();

            var checkValidation = CheckOrderFeedbackValidation(storeOrderList, request);

            if (!checkValidation.IsSuccess)
                return checkValidation;

            try
            {
                var feedbackList = request.FeedbackList.Select(
                    f => BuildToFeedback(f)
                );

                List<Report> reports = new List<Report>();

                var updatedOrderItems = storeOrderList.Select(
                    item =>
                    {
                        var feedback = feedbackList.FirstOrDefault(f => f.OrderItemId == item.Id);

                        if (feedback?.Rating <= MIN_REPORT_RATING) //TODO: REPORTING
                        {
                            item.OrderItemStatus = (int)OrderItemStatusEnum.CLOSED;

                            var title = (item.ProductType == (int)ProductTypeEnum.LAB) ?
                                        "{Email} send a report to playlist of a trainer '{TrainerEmail}'"
                                        .Replace("{Email}", loginUser.Email)
                                        .Replace("{TrainerEmail}", item?.Lab?.CreatedByNavigation?.Email)
                                        : 
                                        "{Email} send a report to product of store '{StoreName}'"
                                        .Replace("{Email}", loginUser.Email)
                                        .Replace("{StoreName}", store.Name);

                            reports.Add(
                                new Report
                                {
                                    Content = feedback.Content,
                                    CreatedBy = loginUserId,
                                    CreatedDate = DateTime.Now,
                                    OrderItemId = feedback.OrderItemId,
                                    Title = title
                                }
                            );
                        }
                        else
                        {
                            item.OrderItemStatus = (int)OrderItemStatusEnum.ORDER_TO_SUCESS;
                        }

                        return item;
                    }
                );

                await unitOfWork.OrderDetailRepository.UpdateAsync(updatedOrderItems);

                _ = unitOfWork.FeedbackRepository.CreateAsync(feedbackList);

                if (!reports.IsNullOrEmpty())
                    _ = unitOfWork.ReportRepository.CreateAsync(reports);

                return ResponseService<object>.OK(
                    new
                    {
                        StoreId = request.SellerId,
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
