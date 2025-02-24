using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public enum OrderStatusEnum
    {
        PENDING = 1,
        DELIVERY = 2,
        PENDING_TO_CUSTOMER_CONFIRM = 3,
        COMPLETED = 4,
        REJECTED = 5,
        CANCELLED_BY_CUSTOMER = 6
    }

    public enum OrderItemStatusEnum
    {
        PENDING = 1,
        PENDING_TO_FEEDBACK = 2,
        COMPLETED = 3,
        CLOSED = 4
    }
}
