using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using System.ComponentModel;

namespace CaptoneProject_IOTS_BOs.DTO.CartDTO
{
    public class AddToCartDTO
    {
        public int ProductId { set; get; }
        public ProductTypeEnum ProductType { set; get; }
        [Range(1, 10000)]
        public int Quantity { set; get; } = 1;
    }

    public class CartLabItemReponseDTO
    {
        public int Id { set; get; }
        public int LabId { set; get; }
        public string Title { set; get; }
        public string Summary { set; get; }
        public decimal Price { set; get; }
    }

    public class CartLabItemDTO
    {
        public bool IsSelected { set; get; } = false;
        public int Id { set; get; }
        public int? LabId { set; get; }
        public string LabName { set; get; }
        public string LabSummary { set; get; }
        public int? CreatedBy { set; get; }
        public string? CreatedByTrainer { set; get; }
        public decimal Price { set; get; }
    }

    public class CartItemResponseDTO
    {
        public bool IsSelected { set; get; } = false;
        public int Id { set; get; }
        public int? IosDeviceId { set; get; }
        public int? ComboId { set; get; }
        public int? CreatedBy { set; get; }
        public string CreatedByStore {set; get;}
        public int ProductType { set; get; }
        public string ProductName { set; get; }
        public string ProductSummary { set; get; }
        public int Quantity { set; get; }
        public decimal Price { set; get; }

        public List<CartLabItemDTO>? labList { set; get; }

        public decimal TotalPrice { set; get; }
    }
}
