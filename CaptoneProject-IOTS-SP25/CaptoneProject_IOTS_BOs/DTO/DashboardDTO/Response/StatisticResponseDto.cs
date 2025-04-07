using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.DashboardDTO.Response
{
    public class StatisticDto
    {
        public int TotalActiveUsers { set; get; }
        public int TotalStoreUsers { set; get; }
        public int TotalTrainerUsers { set; get; }
        public int TotalCustomerUsers { set; get; }
        public int TotalActiveDevices { set; get; }
        public int TotalActiveCombos { set; get; }
        public int TotalLabs { set; get; }
        public int TotalActiveLabs { set; get; }
        public int TotalPendingToApproveLabs { set; get; }
        public int TotalRejectedLabs { set; get; }
        public int TotalOrders { set; get; }
        public int TotalVnPayOrders { set; get; }
        public int TotalIncludedBadFeedbackOrders { set; get; }
        public int TotalCancelledOrders { set; get; }
        public int TotalCashpaymentOrders { set; get; }
        public int TotalSuccessOrders { set; get; }
        public decimal TotalRevenue { set; get; }
    }
}
