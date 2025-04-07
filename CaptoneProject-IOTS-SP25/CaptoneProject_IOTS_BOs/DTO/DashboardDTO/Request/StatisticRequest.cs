using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.DashboardDTO.Request
{
    public class StatisticRequest
    {
        public DateTime StartDate { set; get; }

        public DateTime EndDate { set; get; } = DateTime.Now;
    }
}
