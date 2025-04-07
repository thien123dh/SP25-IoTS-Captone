using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using Microsoft.OData.ModelBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public static class UserEnumConstant
    {
        public enum RoleEnum
        {
            ADMIN = 1,
            STAFF = 2,
            MANAGER = 3,
            TRAINER = 4,
            CUSTOMER = 5,
            STORE = 6
        }

        public static class RoleConst
        {
            public const string ADMIN = "1";
            public const string STAFF = "2";
            public const string MANAGER = "3";
            public const string TRAINER = "4";
            public const string CUSTOMER = "5";
            public const string STORE = "6";

        }
        public enum UserStatusEnum
        {
            INACTIVE = 0,
            ACTIVE = 1,
            PENDING = 2
        }

        public static List<UserStatusDTO> GetUserStatus =>
            new List<UserStatusDTO>
            {
               new UserStatusDTO
               {
                   Id = (int) UserStatusEnum.INACTIVE,
                   Label = "Inactive"
               },
               new UserStatusDTO
               {
                   Id = (int) UserStatusEnum.ACTIVE,
                   Label = "Active"
               },
               new UserStatusDTO
               {
                   Id = (int) UserStatusEnum.PENDING,
                   Label = "Pending"
               }
            };
    }
}
