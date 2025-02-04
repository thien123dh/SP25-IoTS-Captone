using CaptoneProject_IOTS_BOs.DTO.ProductRequestDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class ProductRequestMapper
    {
        private static IMapService<ProductRequest, ProductRequestDTO> productRequestDTOMapper = new MapService<ProductRequest, ProductRequestDTO>();
        
        public static ProductRequestDTO MapToProductRequestDTO(ProductRequest source)
        {
            var res = productRequestDTOMapper.MappingTo(source);

            res.CreatedByEmail = source.CreatedByNavigation.Email;

            if (source.ProductType == (int)ProductTypeEnum.MATERIAL)
            {
                res.ProductName = source?.MaterialNavigation?.Name;
                res.Summary = source?.MaterialNavigation?.Summary;
            }
            else if (source.ProductType == (int)ProductTypeEnum.MATERIAL_GROUP)
            {
                res.ProductName = source?.MaterialGroupNavigation?.Name;
                res.Summary = source?.MaterialGroupNavigation?.Summary;
            }
            else if (source.ProductType == (int)ProductTypeEnum.LAB)
            {
                res.ProductName = source?.LabNavigation?.Title;
                res.Summary = source?.LabNavigation?.Summary;
            }

            return res;
        }
    }
}
