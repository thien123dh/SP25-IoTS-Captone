using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public enum OrderStatusEnum
    {
        SUCCESS_TO_ORDER = 1,
        REFUNDED = 2
    }

    public enum OrderItemStatusEnum
    {
        PENDING = 1,
        PENDING_TO_PACKING = 2,
        PENDING_TO_DELEVERING = 3,
        PENDING_TO_DELEVERED = 4,
        PENDING_TO_FEEDBACK = 5,
        ORDER_TO_SUCESS = 6,
        CLOSED = 7
    }
}
