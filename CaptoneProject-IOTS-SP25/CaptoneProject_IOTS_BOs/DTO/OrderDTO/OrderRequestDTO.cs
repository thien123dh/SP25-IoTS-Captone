﻿using CaptoneProject_IOTS_BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.OrderDTO
{
    public class OrderRequestDTO
    {
        [MaxLength(500)]
        public string Address { set; get; } = "";

        [MaxLength(150)]
        public string ContactNumber { set; get; } = "";

        [MaxLength(300)]
        public string? Notes { set; get; } = "";
    }
}
