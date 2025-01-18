using CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public static class ActivityLogContanst
    {
        private static ActivityLogEntityTypeDTO CreateEntityLogTypeDTO(int id, string label, int isActive)
        {
            return new ActivityLogEntityTypeDTO
            {
                Id = id,
                label = label,
                IsActive = isActive
            };
        }
        public static IEnumerable<ActivityLogEntityTypeDTO> GetAllActivityLogEntityTypes()
        {
            return [
                CreateEntityLogTypeDTO((int) ActivityLogEntityTypeEnum.USER_REQUEST, "User Request", 0),
                CreateEntityLogTypeDTO((int) ActivityLogEntityTypeEnum.USER, "User", 1),
                CreateEntityLogTypeDTO((int) ActivityLogEntityTypeEnum.MATERIAL, "Material", 1),
                CreateEntityLogTypeDTO((int) ActivityLogEntityTypeEnum.MATERIAL_GROUP, "Material Group", 1)
            ];
        }
        public enum ActivityLogEntityTypeEnum
        {
            USER_REQUEST = 1,
            USER = 2,
            MATERIAL = 3,
            MATERIAL_GROUP = 4,
        }

    }

    public static class ActivityLogMessageConstant
    {
        public static string ACTIVITY_LOG_MESSAGE_TEMPLATE = "{S} {Action} {Target}";
    }
}
