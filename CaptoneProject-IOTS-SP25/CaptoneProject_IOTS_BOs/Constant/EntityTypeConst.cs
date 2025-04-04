using CaptoneProject_IOTS_BOs.DTO.ActivityLogDTO;

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
                CreateEntityLogTypeDTO((int) EntityTypeEnum.IOT_DEVICE, "Material", 1),
                CreateEntityLogTypeDTO((int) EntityTypeEnum.IOT_DEVICE_COMBO, "Material Group", 1)
            ];
        }
        public enum EntityTypeEnum
        {
            USER_REQUEST = 1,
            USER = 2,
            IOT_DEVICE = 3,
            IOT_DEVICE_COMBO = 4,
            LAB = 5,
            BLOG = 6, 
            TRANSACTION = 7,
            WARRANTY_REQUEST = 8,
            ORDER = 9,
            CASHOUT_REQUEST = 10,
            REFUND_REQUEST = 11,
            REPORT = 12
        }
    }

    public static class ActivityLogMessageConstant
    {
        public static string ACTIVITY_LOG_MESSAGE_TEMPLATE = "{S} {Action} '{Target}'";
    }
}
