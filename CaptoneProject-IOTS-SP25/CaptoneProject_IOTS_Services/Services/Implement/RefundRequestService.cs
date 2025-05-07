using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class RefundRequestService : IRefundRequestService
    {
        private readonly IUserServices userServices;
        private readonly UnitOfWork unitOfWork;
        private readonly IActivityLogService activityLogService;

        public RefundRequestService(IUserServices userServices, 
            UnitOfWork unitOfWork, 
            IActivityLogService activityLogService)
        {
            this.userServices = userServices;
            this.unitOfWork = unitOfWork;
            this.activityLogService = activityLogService;
        }

        public async Task<ResponseDTO> GetPaginationRefundRequest(int? statusFilter, PaginationRequest request)
        {
            var loginUserId = userServices.GetLoginUserId();
            int role = (int)userServices.GetRole();

            var adminOrStaff = new List<int> { (int)RoleEnum.ADMIN, (int)RoleEnum.STAFF };

            Expression<Func<RefundRequest, bool>> func = item => (statusFilter == null || statusFilter == item.Status) && item.CreatedBy == loginUserId;
            
            if (adminOrStaff.Contains((int)role))
            {
                func = item => (statusFilter == null || statusFilter == item.Status);
            }

            var pagination = unitOfWork.RefundRequestRepository.GetPaginate(
                filter: func,
                includeProperties: "CreatedByNavigation,ActionByNavigation",
                orderBy: ob => ob.OrderByDescending(item => item.CreatedDate),
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            return ResponseService<object>.OK(pagination);
        }

        public async Task<ResponseDTO> UpdateStatusToHandled(int requestId)
        {
            var loginUserId = userServices.GetLoginUserId();
            var refund = unitOfWork.RefundRequestRepository.GetById(requestId);

            if (refund == null)
                return ResponseService<object>.NotFound("The refund request cannot be found. Please try again");

            if (refund.Status != (int)RefundRequestStatusEnum.PENDING_TO_HANDLE)
                return ResponseService<object>.BadRequest("The refund request has been already handled. Please check again");

            var order = unitOfWork.OrderRepository.GetById(refund.OrderId);

            refund.Status = (int)RefundRequestStatusEnum.HANDLED;
            refund.ActionDate = DateTime.Now;
            refund.ActionBy = (int)loginUserId;

            refund = unitOfWork.RefundRequestRepository.Update(refund);

            Notifications noti = new Notifications
            {
                Content = $"Your refund request of order {order.ApplicationSerialNumber} has been Handled",
                Title = $"Your refund request of order {order.ApplicationSerialNumber} has been Handled",
                CreatedDate = DateTime.Now,
                EntityId = refund.Id,
                EntityType = (int)EntityTypeConst.EntityTypeEnum.REFUND_REQUEST,
                ReceiverId = refund.CreatedBy
            };

            _ = unitOfWork.NotificationRepository.Create(noti);

            _ = activityLogService.CreateActivityLog($"Changed status of refund request ID {requestId} to Handled");

            return ResponseService<object>.OK(refund);

        }
    }
}
