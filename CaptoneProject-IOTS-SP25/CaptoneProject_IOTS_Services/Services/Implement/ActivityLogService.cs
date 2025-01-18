using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
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
        private readonly MyHttpAccessor httpAccessor;
        public ActivityLogService (
            ActivityLogRepository activityLogRepository,
            MyHttpAccessor httpAccessor
        )
        {
            this.activityLogRepository = activityLogRepository;
            this.httpAccessor = httpAccessor;
        }

        public async Task<ResponseDTO> CreateActivityLog(CreateActivityLogDTO source)
        {
            ActivityLog activityLog = new ActivityLog
            {
                EntityId = source.EntityId,
                EntityType = source.EntityType,
                Title = source.Title,
                Contents = source.Contents,
                CreatedBy = httpAccessor.GetLoginUserId(),
                CreatedDate = DateTime.Now,
                Metadata = source.MetaData
            };

            activityLogRepository.Create(activityLog);

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }

        public Task<ResponseDTO> CreateUserHistoryTrackingActivityLog(string action, string target, string? metaData)
        {
            User loginUser = httpAccessor.GetLoginUser();

            string message = ActivityLogMessageConstant.ACTIVITY_LOG_MESSAGE_TEMPLATE
                    .Replace("{S}", loginUser?.Fullname).Replace("{Action}", action).Replace("{Target}", target);

            return CreateActivityLog(
                new CreateActivityLogDTO
                {
                    EntityId = loginUser.Id,
                    EntityType = (int)ActivityLogContanst.ActivityLogEntityTypeEnum.USER,
                    Title = message,
                    Contents = message,
                    MetaData = metaData
                }
            );
        }

        public ResponseDTO GetAllActivityLogTypes()
        {
            return new ResponseDTO
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = ActivityLogContanst.GetAllActivityLogEntityTypes().Where(type => type.IsActive > 0)
            };
        }

        public async Task<ResponseDTO> GetPaginationActivityLog(
            PaginationRequest payload, 
            int? entityId, 
            int? entityType, 
            int? userId)
        {
            PaginationResponse<ActivityLog> pagination = activityLogRepository.GetPaginate(
                filter: a => (
                    (entityId == null || a.EntityId == entityId)
                    &&
                    (entityType == null || a.EntityType == entityType)
                    &&
                    (userId == null || a.CreatedBy == userId)
                ),
                orderBy: null,
                includeProperties: null,
                pageIndex: payload.PageIndex,
                pageSize: payload.PageSize
            );

            if (pagination == null)
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "No Data Result",
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = PaginationMapper<ActivityLog, ActivityLogResponseDTO>.mappingTo(ActivityLogMapper.mappingToActivityLogResponseDTO, pagination)
            };
        }
    }
}
