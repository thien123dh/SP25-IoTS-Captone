using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public static class ProductRequestConst
    {
        public enum ProductRequestStatusEnum
        {
            PENDING_TO_APPROVED = 1,
            APPROVED = 2,
            REJECTED = 3
        }
    }
}
