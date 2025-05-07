using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.RefundDTO
{
    public class CreateRefundRequestDTO
    {

        public string ContactNumber { set; get; }

        public string AccountName { set; get; }

        public string AccountNumber { set; get; }

        public string BankName { set; get; }
    }

    public class RefundRequestDTO
    {
        public int Id { set; get; }
        public decimal Amount { set; get; }
        public int OrderId { set; get; }
        public string OrderCode { set; get; }
        public string ContactNumber { set; get; }

        public string AccountName { set; get; }

        public string AccountNumber { set; get; }

        public string BankName { set; get; }
        public int CreatedBy { set; get; }
        public User CreatedByNavigation { set; get; }
        public DateTime CreatedDate { set; get; }
        public int ActionBy { set; get; }

        public User ActionByNavigation { set; get; }

        public DateTime ActionDate { set; get; }

        public short Status { set; get; }
    }
}
