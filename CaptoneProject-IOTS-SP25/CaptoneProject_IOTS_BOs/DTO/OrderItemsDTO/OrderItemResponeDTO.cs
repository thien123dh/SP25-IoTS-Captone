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
    public class OrderItemResponeDTO
    {
        public string NameShop { set; get; }

        public int ProductType { set; get; }

        public int Quantity { set; get; } = 0;

        public decimal Price { set; get; }

        public DateTime? WarrantyEndDate { set; get; } = null;

        public int OrderItemStatus { set; get; } = 1;
    }

    public class OrderItemResponeUserDTO
    {
        public string NameProduct { get; set; }

        public string? ImageUrl { set; get; }

        public int ProductType { set; get; }

        public int Quantity { set; get; } = 0;

        public decimal Price { set; get; }

        public DateTime? WarrantyEndDate { set; get; } = null;

        public int OrderItemStatus { set; get; } = 1;
    }

    public class SellerOrderDetailsDTO
    {
        public string SellerName { get; set; }
        public List<OrderItemResponeUserDTO> Items { get; set; }
    }
}
