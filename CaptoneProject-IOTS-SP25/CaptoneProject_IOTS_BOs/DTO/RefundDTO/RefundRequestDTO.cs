using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.RefundDTO
{
    public class CreateRefundRequestDTO
    {

        public string ContactNumber { set; get; }

        public string AccountName { set; get; }

        public string AccountNumber { set; get; }

        public string BankName { set; get; }
    }
}
