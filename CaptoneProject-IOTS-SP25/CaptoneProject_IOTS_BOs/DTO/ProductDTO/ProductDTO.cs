using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_BOs.DTO.ProductDTO
{
    public class GeneralProductDTO
    {
        public string? Name { set; get; }
        public string? Summary { set; get; }
        public decimal Price { set; get; }
        public int? CreatedBy { set; get; }
        public string? CreatedByStore { set; get; }

        public string? ImageUrl { set; get; }
    }

    public class ProductRequestDTO
    {
        public int ProductId { set; get; }

        public ProductTypeEnum ProductType { set; get; }
    }
}
