using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.BlogDTO
{
    public class CreateUpdateBlogDTO
    {
        public string Title { get; set; }

        public string BlogContent { get; set; }

        public int? CategoryId { get; set; }

        public string MetaData { get; set; }
    }
}
