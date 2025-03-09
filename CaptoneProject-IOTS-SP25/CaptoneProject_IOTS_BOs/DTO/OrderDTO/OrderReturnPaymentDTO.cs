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
    public class OrderReturnPaymentDTO
    {
        public string PaymentUrl { get; set; }

        public string ApplicationSerialNumber { set; get; }

        public decimal TotalPrice { set; get; } = 0;

        public string Address { set; get; } = "";
        public int ProvinceId { set; get; }
        public string ProvinceName { set; get; }
        public int DistrictId { set; get; }
        public string DistrictName { set; get; }
        public int WardId { set; get; }
        public string WardName { set; get; }
        public string ContactNumber { set; get; } = "";

        public string Notes { set; get; }

        public DateTime CreateDate { set; get; }

        public DateTime UpdatedDate { set; get; }

        public int OrderStatusId { set; get; }
    }
}
