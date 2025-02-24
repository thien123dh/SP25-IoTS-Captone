using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
   public static class ProductConst
   {
        public enum ProductTypeEnum
        {
            IOT_DEVICE = 1,
            COMBO = 2,
            LAB = 3
        }

        public enum ProductStatusEnum
        {
            INACTIVE = 0,
            ACTIVE = 1,
            PENDING = 2
        }

        public enum IotDeviceTypeEnum
        {
            NEW = 1,
            SECOND_HAND = 2
        }

        public enum LabStatusEnum
        {
            DRAFT = 0,
            APPROVED = 1,
            PENDING_TO_APPROVE = 2,
            REJECTED = 3
        }
   }
}
