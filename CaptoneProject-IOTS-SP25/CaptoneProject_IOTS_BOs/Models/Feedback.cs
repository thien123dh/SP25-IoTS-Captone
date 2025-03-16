using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public partial class Feedback
    {
        public int Id { set; get; }

        public string Content { set; get; }

        [ForeignKey(nameof(User))]
        public int? CreatedBy { set; get; }

        public DateTime? CreatedDate { set; get; }

        public int OrderItemId { set; get; }

        [ForeignKey(nameof(OrderItemId))]
        [JsonIgnore]
        public OrderItem OrderItem { set; get; }

        [Precision(9, 1)]
        public decimal Rating { set; get; }
    }
}
