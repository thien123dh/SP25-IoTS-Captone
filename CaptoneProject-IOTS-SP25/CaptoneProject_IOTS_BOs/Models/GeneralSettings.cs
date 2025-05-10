using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public class GeneralSettings
    {
        public int Id { set; get; }

        public int ApplicationFeePercent { set; get; }

        public int OrderSuccessDays { set; get; }

        public DateTime UpdatedDate { set; get; }

        public int UpdatedBy { set; get; }
    }
}
