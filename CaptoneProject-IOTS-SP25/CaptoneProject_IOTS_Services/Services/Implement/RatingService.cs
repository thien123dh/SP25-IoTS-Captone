using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.RatingDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class RatingService : IRatingService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
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

        public Task<ResponseDTO> GetReportPagination(PaginationRequest request)
        {
            throw new NotImplementedException();
        }

        public Feedback BuildFeedback(RatingRequestDTO source, int? loginUserId = null)
        {
            var model = GenericMapper<RatingRequestDTO, Feedback>.MapTo(source);

            model.CreatedDate = DateTime.Now;
            model.CreatedBy = loginUserId;

            return model;
        }

        public async Task<ResponseDTO> RatingProduct(List<RatingRequestDTO> request)
        {
            var loginUserId = userServices.GetLoginUserId();

            var numberOfOrderItemFeedback = unitOfWork.OrderDetailRepository.Search(
                item => request.Count(item => item.OrderItemId == item.OrderItemId) > 0
                    && item.OrderItemStatus == (int)OrderItemStatusEnum.COMPLETED
            )?.Count();

            if (numberOfOrderItemFeedback != request.Count())
                return ResponseService<object>.BadRequest("You might already send your feedback. Please check again");

            try
            {

                var saveFeedbackList = request.Select(item => BuildFeedback(item, loginUserId));

                var saveReport = request.Where(item => item.Rating <= 2);

                if (saveFeedbackList != null)
                    await unitOfWork.FeedbackRepository.CreateAsync(saveFeedbackList);

                if (saveReport != null)
                    ;

                return ResponseService<object>.OK(null);

            } catch (Exception ex)
            {
                return ResponseService<object>.BadRequest("You cannot send your feedback. Please try again");
            }
        }
    }
}
