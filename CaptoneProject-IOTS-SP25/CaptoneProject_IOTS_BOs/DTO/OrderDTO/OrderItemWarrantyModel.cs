using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.OrderDTO
{
    public class CreateOrderWarrantyInfo
    {
        public List<OrderItemWarrantyModel>? OrderProductInfo { set; get; }
    }

    public class OrderItemWarrantyModel
    {
        public int OrderItemId { set; get; }

        public List<string>? PhysicalSerialNumber { set; get; }
    }
}
