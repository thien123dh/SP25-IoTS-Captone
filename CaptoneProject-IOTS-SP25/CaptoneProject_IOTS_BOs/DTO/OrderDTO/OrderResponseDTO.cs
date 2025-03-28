using CaptoneProject_IOTS_BOs.DTO.OrderItemsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.OrderDTO
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public string ApplicationSerialNumber { set; get; }

        public decimal TotalPrice { set; get; } = 0;

        public string Address { set; get; } = "";

        public string ContactNumber { set; get; } = "";

        public string Notes { set; get; }

        public DateTime CreateDate { set; get; }

        public DateTime UpdatedDate { set; get; }

        public int OrderStatusId { set; get; }

        public decimal? ShippingFee { set; get; }

        public int TotalCount { set; get; }

        public int PendingNumber { set; get; }

        public int DeliveringNumber { set; get; }

        public int PendingToFeedbackNumber { set; get; }

        public List<OrderItemsGroupResponseDTO>? OrderDetailsGrouped { get; set; }
    }
}
