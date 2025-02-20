using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.BlogDTO
{
    public class BlogResponeDTO
    {
        public string Title { get; set; }

        public string BlogContent { get; set; }

        public int? CategoryId { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? IsActive { get; set; }

        public string MetaData { get; set; }

        public virtual ICollection<BlogAttachment> BlogAttachments { get; set; } = new List<BlogAttachment>();

        public virtual ICollection<BlogFeedback> BlogFeedbacks { get; set; } = new List<BlogFeedback>();

        public virtual BlogsCategory Category { get; set; }

        public virtual User CreatedByNavigation { get; set; }

        public virtual User UpdatedByNavigation { get; set; }
    }
}
