using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.CashoutRequestDTO
{
    public class CreateCashoutRequestDTO
    {
        public decimal Amount { set; get; }

        public string ContactNumber { set; get; }

        public string AccountName { set; get; }

        public string AccountNumber { set; get; }

        public string BankName { set; get; }
    }

    public class CashoutRequestResponseDTO
    {
        public decimal Amount { set; get; }

        public string ContactNumber { set; get; }

        public string AccountName { set; get; }

        public string AccountNumber { set; get; }

        public string BankName { set; get; }

        public int CreatedBy { set; get; }

        public string CreatedByNavigationFullname { set; get; }

        public int ActionBy { set; get; }

        public string ActionByNavigationFullname { set; get; }

        public short Status { set; get; }
    }
}
