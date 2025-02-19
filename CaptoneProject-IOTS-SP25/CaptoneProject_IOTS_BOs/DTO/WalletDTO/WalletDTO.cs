using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.WalletDTO
{
    public class CreateUpdateWalletDTO
    {
        public int UserId { set; get; }
        public decimal Ballance { set; get; }
    }

    public class CreateTransactionWalletDTO
    {
        public int UserId { set; get; }
        public decimal Amount { set; get; }
        public string Description { set; get; }
        public string TransactionType { set; get; }
    }

    public class SendCurrencyToUserDTO
    {
        public int UserId { set; get; }
        public decimal Amount { set; get; }
    }
}
