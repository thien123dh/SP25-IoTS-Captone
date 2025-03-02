using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.OrderItemsDTO
{
    public class OrderItemsRequestDTO
    {

        public int OrderId { set; get; }

        public int? IosDeviceId { set; get; }

        public int? ComboId { set; get; }

        public int? LabId { set; get; }

        public int SellerId { set; get; }

        public int OrderBy { set; get; }

        public int ProductType { set; get; }

        public int Quantity { set; get; } = 0;

        public int Price { set; get; }

        public DateTime? WarrantyEndDate { set; get; } = null;

        public int OrderItemStatus { set; get; } = 1;
    }
}
