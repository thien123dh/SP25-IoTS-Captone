using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using MailKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class ActivityLogMapper
    {
        private readonly static IMapService<ActivityLog, ActivityLogResponseDTO> activityLogMapper = new MapService<ActivityLog, ActivityLogResponseDTO>();

        public static ActivityLogResponseDTO mappingToActivityLogResponseDTO(ActivityLog source)
        {
            ActivityLogResponseDTO res = activityLogMapper.MappingTo(source);

            res.EntityTypeLabel = ActivityLogContanst.GetAllActivityLogEntityTypes()?.SingleOrDefault(type => type.Id == res.EntityType)?.label;

            return res;
        }
    }
}
