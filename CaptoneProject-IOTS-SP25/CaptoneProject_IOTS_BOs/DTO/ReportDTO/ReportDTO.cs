using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_BOs.DTO.ReportDTO
{
    public class DtoRefundReportRequest
    {
        public int RefundQuantity { set; get; }
    }

    public class ReportResponseDTO
    {
        public int Id { set; get; }

        public string? Title { set; get; }

        public int OrderItemId { set; get; }

        public string? OrderCode { set; get; }

        public int? ProductId { set; get; }

        public int? ProductType { set; get; }

        public string? ProductName { set; get; }

        public string? Content { set; get; }

        public int? CreatedBy { set; get; }

        public string? CreatedByEmail { set; get; }

        public string? CreatedByFullname { set; get; }

        public string? ContactNumber { set; get; }

        public int? StoreId { set; get; }

        public string? StoreName { set; get; }

        public DateTime? CreatedDate { set; get; }

        public short Status { set; get; }
    }
}
