using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.Models
{
    public class WarrantyRequest
    {
        public int Id { set; get; }

        public string Description { set; get; }

        public string OrderItemId { set; get; }

        public string Remarks { set; get; }

        public int Status { set; get; }

        public string IdentifySerialNumber { set; get; }

        public string Address { set; get; }

        public string ContactNumber { set; get; }

        [ForeignKey(nameof(User))]
        public int CreatedBy { set; get; }

        public DateTime CreatedDate { set; get; } = DateTime.Now;

        public DateTime ActionDate { set; get; } = DateTime.Now;
        public virtual List<WarrantyRequestAttachments> WarrantyRequestAttachments { set; get; }
    }
}
