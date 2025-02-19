using CaptoneProject_IOTS_BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.Constant;

namespace CaptoneProject_IOTS_BOs.DTO.TransactionDTO
{
    public class CreateTransactionDTO
    {
        public int UserId { set; get; }

        [Precision(18, 1)]
        public decimal Amount { set; get; }

        public string TransactionType { set; get; }

        [MaxLength(250)]
        public string Description { set; get; }

        public string Status { set; get; }
    }
}
