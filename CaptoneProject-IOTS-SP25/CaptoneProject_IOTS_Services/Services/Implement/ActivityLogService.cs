using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly ActivityLogRepository activityLogRepository;
        private readonly IUserServices userServices;
        public ActivityLogService(
            ActivityLogRepository activityLogRepository,
            IUserServices userServices
        )
        {
            this.activityLogRepository = activityLogRepository;
            this.userServices = userServices;
        }

        public async Task<ResponseDTO> CreateActivityLog(string message)
        {
            try
            {
                User loginUser = userServices.GetLoginUser();

                ActivityLog activityLog = new ActivityLog
                {
                    Title = message,
                    Content = message,
                    UserId = loginUser == null ? 0 : loginUser.Id
                };

                activityLogRepository.Create(activityLog);

                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK,
                };

            } catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = true,
                    Message = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                };
            }
        }

        public async Task<ResponseDTO> GetPaginationActivityLog(PaginationRequest payload, int? userId)
        {
            var res = activityLogRepository.GetPaginate(
                filter: item => item.UserId == userId &&
                        ((payload.StartFilterDate == null || payload.StartFilterDate <= item.CreatedDate) && (payload.EndFilterDate == null || payload.EndFilterDate >= item.CreatedDate)),
                orderBy: ob => ob.OrderByDescending(o => o.CreatedDate),
                includeProperties: "",
                pageIndex: payload.PageIndex,
                pageSize: payload.PageSize
            );

            return ResponseService<object>.OK(res);
        }
    }
}
