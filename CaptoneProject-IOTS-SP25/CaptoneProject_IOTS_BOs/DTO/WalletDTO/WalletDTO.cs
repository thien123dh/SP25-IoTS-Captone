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
}
