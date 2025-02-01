using CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public static class EntityTypeConst
    {
        private static EntityTypeDTO CreateEntityLogTypeDTO(int id, string label, int isActive)
        {
            return new EntityTypeDTO
            {
                Id = id,
                label = label,
                IsActive = isActive
            };
        }
        public static IEnumerable<EntityTypeDTO> GetAllActivityLogEntityTypes()
        {
            return [
                CreateEntityLogTypeDTO((int) EntityTypeEnum.USER_REQUEST, "User Request", 0),
                CreateEntityLogTypeDTO((int) EntityTypeEnum.USER, "User", 1),
                CreateEntityLogTypeDTO((int) EntityTypeEnum.MATERIAL, "Material", 1),
                CreateEntityLogTypeDTO((int) EntityTypeEnum.MATERIAL_GROUP, "Material Group", 1)
            ];
        }
        public enum EntityTypeEnum
        {
            USER_REQUEST = 1,
            USER = 2,
            MATERIAL = 3,
            MATERIAL_GROUP = 4,
            BLOG = 5
        }
    }

    public static class ActivityLogMessageConstant
    {
        public static string ACTIVITY_LOG_MESSAGE_TEMPLATE = "{S} {Action} {Target}";
    }
}
