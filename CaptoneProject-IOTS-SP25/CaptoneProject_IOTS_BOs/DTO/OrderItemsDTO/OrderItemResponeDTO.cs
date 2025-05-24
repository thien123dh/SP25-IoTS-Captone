﻿using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

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

    public class OrderItemResponseDTO
    {
        public int OrderItemId { set; get; }
        public string? ImageUrl { set; get; }
        public int? ProductId { set; get; }
        public string? NameProduct { get; set; }
        public int ProductType { set; get; }

        public int Quantity { set; get; } = 0;

        public decimal Price { set; get; }

        public DateTime? WarrantyEndDate { set; get; } = null;
        public int WarrantyMonths { set; get; }
        public DateTime? UpdatedDate { set; get; }
        public int OrderItemStatus { set; get; } = 1;
        public List<string>? PhysicalSerialNumbers { set; get; }
        public short? ReportStatus { set; get; }

        public int? RefundQuantity { set; get; }

        public decimal? RefundAmount { set; get; }
    }

    public class OrderItemsGroupResponseDTO
    {
        public int? StoreId { set; get; }
        public string? SellerName { set; get; }
        public int? SellerId { set; get; }
        public int SellerRole { set; get; }
        public virtual string SellerRoleName => this.SellerRole == (int)RoleEnum.STORE ? "Store" : "Trainer";
        public string TrackingId { get; set; }
        public int WarrantyMonths { set; get; }
        public int? OrderItemStatus { set; get; }
        public decimal? TotalAmount { set; get; }
        public List<OrderItemResponseDTO>? Items { get; set; }
    }
}
