using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Constant
{
    public static class TransactionTypeEnum
    {
        public const string SYSTEM_REWARD = "System Reward";
        public const string SUCCESS_ORDER = "Success Order";
    }

    public static class TransactionStatusEnum
    {
        public const string FAILED = "Failed";
        public const string SUCCESS = "Success";
        public const string PENDING = "Pending";
    }
}
