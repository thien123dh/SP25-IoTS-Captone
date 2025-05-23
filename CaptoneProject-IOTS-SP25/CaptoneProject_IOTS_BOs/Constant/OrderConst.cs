﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public enum OrderStatusEnum
    {
        PENDING_TO_ORDER = 0,
        SUCCESS_TO_ORDER = 1,
        CANCELLED = 2,
        CASH_PAYMENT = 3
    }

    public enum OrderItemStatusEnum
    {
        PENDING = 1,
        PACKING = 2,
        DELEVERING = 3,
        DELEVERED = 4,
        PENDING_TO_FEEDBACK = 5,
        SUCCESS_ORDER = 6,
        CANCELLED = 7,
        BAD_FEEDBACK = 8
    }
}
