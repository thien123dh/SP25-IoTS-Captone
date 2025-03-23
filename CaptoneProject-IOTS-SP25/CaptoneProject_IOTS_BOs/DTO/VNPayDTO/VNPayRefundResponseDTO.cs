using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.VNPayDTO
{
    public class VNPayRefundResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> Data { get; set; }

        public VNPayRefundResponseDTO()
        {
            Data = new Dictionary<string, string>();
        }
    }

    public class RefundRequestDTO
    {
        public string? TxnRef { get; set; }

        public long amount { get; set; }

        [Url]
        public string urlResponse { get; set; } = null!;
    }

}
