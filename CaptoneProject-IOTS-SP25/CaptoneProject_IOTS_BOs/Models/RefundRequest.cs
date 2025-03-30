using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class RefundRequest
    {
        public int Id { set; get; }
        public decimal Amount { set; get; }
        public int OrderId { set; get; }
        public string ContactNumber { set; get; }

        public string AccountName { set; get; }

        public string AccountNumber { set; get; }

        public string BankName { set; get; }
        [ForeignKey(nameof(User))]
        public int CreatedBy { set; get; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual User CreatedByNavigation { set; get; }
        public DateTime CreatedDate { set; get; }


        [ForeignKey(nameof(User))]
        public int ActionBy { set; get; }

        [ForeignKey(nameof(ActionBy))]
        public virtual User ActionByNavigation { set; get; }

        public DateTime ActionDate { set; get; }

        public short Status { set; get; }
    }
}
