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
        private readonly IUserServices userServices;
        public ActivityLogService (
            ActivityLogRepository activityLogRepository,
            IUserServices userServices
        )
        {
            this.activityLogRepository = activityLogRepository;
            this.userServices = userServices;
        }

        public async Task<ResponseDTO> CreateActivityLog(CreateActivityLogDTO source)
        {
            User loginUser = userServices.GetLoginUser();

            //if (loginUser == null)
            //{
            //    return new ResponseDTO
            //    {
            //        IsSuccess = false,
            //        Message = "Cannot create activity log",
            //        StatusCode = System.Net.HttpStatusCode.BadRequest
            //    };
            //}

            ActivityLog activityLog = new ActivityLog
            {
                EntityId = source.EntityId,
                EntityType = source.EntityType,
                Title = source.Title,
                Contents = source.Contents,
                CreatedBy = loginUser == null ? 0 : loginUser.Id
                //CreatedDate = DateTime.Now
            };

            activityLogRepository.Create(activityLog);

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }

        public async Task<ResponseDTO> CreateUserHistoryTrackingActivityLog(string action, string target, string? metaData)
        {
            try
            {
                User loginUser = userServices.GetLoginUser();

                if (loginUser == null)
                    return new ResponseDTO();

                string message = ActivityLogMessageConstant.ACTIVITY_LOG_MESSAGE_TEMPLATE
                        .Replace("{S}", loginUser?.Fullname).Replace("{Action}", action).Replace("{Target}", target);

                return await CreateActivityLog(
                    new CreateActivityLogDTO
                    {
                        EntityId = loginUser.Id,
                        EntityType = (int)EntityTypeConst.EntityTypeEnum.USER,
                        Title = message,
                        Contents = message,
                        MetaData = ""
                    }
                );
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            
        }

        public ResponseDTO GetAllActivityLogTypes()
        {
            return new ResponseDTO
            {
                IsSuccess = true,
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = EntityTypeConst.GetAllActivityLogEntityTypes().Where(type => type.IsActive > 0)
            };
        }

        public async Task<ResponseDTO> GetPaginationActivityLog(
            PaginationRequest payload, 
            int? entityId, 
            int? entityType, 
            int? userId)
        {
            PaginationResponseDTO<ActivityLog> pagination = activityLogRepository.GetPaginate(
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
