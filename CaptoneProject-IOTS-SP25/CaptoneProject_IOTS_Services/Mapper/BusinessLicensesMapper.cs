using CaptoneProject_IOTS_BOs.DTO.StoreDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class BusinessLicensesMapper
    {
        private static IMapService<BusinessLicensesDTO, BusinessLicenses> businessLicensesMapper = new MapService<BusinessLicensesDTO, BusinessLicenses>();
        public static BusinessLicenses MapToBusinessLicenses(BusinessLicensesDTO source)
        {
            return businessLicensesMapper.MappingTo(source);
        }
    }
}
