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
            MATERIAL = 1,
            MATERIAL_GROUP = 2,
            LAB = 3
        }

        public enum ProductStatusEnum
        {
            INACTIVE = 0,
            ACTIVE = 1,
            PENDING = 2
        }
   }
}
