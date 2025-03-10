using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.OrderItemsDTO
{
    public class OrderIstemResponseToStoreDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? IosDeviceId { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string IosDeviceName { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ComboId { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ComboName { get; set; }
        public int SellerId { get; set; }
        public int ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public int OrderItemStatus { get; set; }
    }
}
