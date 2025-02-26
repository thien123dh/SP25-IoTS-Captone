using CaptoneProject_IOTS_BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.DTO.OrderItemsDTO;

namespace CaptoneProject_IOTS_BOs.DTO.OrderDTO
{
    public class OrderResponeDTO
    {
        [MaxLength(200)]
        public string ApplicationSerialNumber { set; get; }

        [ForeignKey(nameof(User))]
        public int SellerId { set; get; }

        [ForeignKey(nameof(User))]
        public int OrderBy { set; get; }

        [Precision(18, 1)]
        public decimal TotalPrice { set; get; } = 0;

        [MaxLength(500)]
        public string Address { set; get; } = "";

        [MaxLength(150)]
        public string ContactNumber { set; get; } = "";

        [MaxLength(300)]
        public string? Notes { set; get; } = "";

        [MaxLength(300)]
        public string? Remark { set; get; } = "";

        public DateTime CreateDate { set; get; } = DateTime.Now;

        [ForeignKey(nameof(User))]
        public int? CreatedBy { set; get; }

        public DateTime UpdatedDate { set; get; } = DateTime.Now;

        [ForeignKey(nameof(User))]
        public int? UpdatedBy { set; get; }

        [Range(1, 15)]
        public int OrderStatusId { set; get; }

        public List<OrderItemResponeDTO> Details { get; set; }
    }
}
