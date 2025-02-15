using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.VNPayDTO
{
    public class ResponsePayment
    {
        public string? ResponseCodeMessage { get; set; }
        public string? TransactionStatusMessage { get; set; }

        public VnPayResponse? VnPayResponse { get; set; }
    }
}
