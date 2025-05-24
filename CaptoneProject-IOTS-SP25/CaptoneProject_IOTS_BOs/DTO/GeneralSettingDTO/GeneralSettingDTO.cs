using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.GeneralSettingDTO
{
    public class UpdateGeneralSettingRequest
    {
        public int Id { set; get; }

        public int ApplicationFeePercent { set; get; }

        public int OrderSuccessDays { set; get; }
    }
}
