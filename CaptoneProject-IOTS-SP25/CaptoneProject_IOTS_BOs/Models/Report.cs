using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Report
    {
        public int Id { set; get; }

        public string Title { set; get; }

        [ForeignKey(nameof(OrderItem))]
        public int OrderItemId { set; get; }

        [ForeignKey(nameof(OrderItemId))]
        public OrderItem? OrderItem { set; get; }

        public string Content { set; get; }

        public decimal? RefundAmount { set; get; }

        public int? RefundQuantity { set; get; }

        public string? BankName { set; get; }

        public string? AccountName { set; get; }

        public string? AccountNumber { set; get; }

        [ForeignKey(nameof(User))]
        public int? CreatedBy { set; get; }

        [ForeignKey(nameof(CreatedBy))]
        public User? CreatedByNavigation { set; get; }

        public DateTime? CreatedDate { set; get; }

        public short Status { set; get; }
    }
}
