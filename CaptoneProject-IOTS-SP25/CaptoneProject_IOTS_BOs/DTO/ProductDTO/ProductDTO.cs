using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.ProductDTO
{
    public class GeneralProductDTO
    {
        public string ProductName { set; get; }

        public string ProductSummary { set; get; }
        public decimal Price { set; get; }
        public int? CreatedBy { set; get; }
    }
}
