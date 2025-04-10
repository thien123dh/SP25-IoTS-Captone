using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.DashboardDTO.Request
{
    public class StatisticRequest
    {
        public int Year { set; get; }

        public int? Month { set; get; }
    }

    public class StatisticRequestRange
    {
        public DateTime StartDate { set; get; }

        public DateTime EndDate { set; get; }

        public List<DtoTimesStartEndDate> TimeList { set; get; } = new List<DtoTimesStartEndDate>();
    }

    public class DtoTimesStartEndDate
    {
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public string Time { set; get; }
    }
}
