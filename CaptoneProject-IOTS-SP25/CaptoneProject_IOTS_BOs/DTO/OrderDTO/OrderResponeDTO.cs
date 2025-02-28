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
        public string PaymentUrl { get; set; }
    }
}
