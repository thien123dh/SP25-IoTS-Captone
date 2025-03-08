using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
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
    public static class IotDeviceMapper
    {
        private static readonly IMapService<CreateUpdateIotDeviceDTO, IotsDevice> iotSaveMapper = new MapService<CreateUpdateIotDeviceDTO, IotsDevice>();

        public static IotDeviceDetailsDTO MapToIotDeviceDetailsDTO(IotsDevice source)
        {
            var res = GenericMapper<IotsDevice, IotDeviceDetailsDTO>.MapTo(source);

            res.DeviceSpecificationsList = source?.DeviceSpecifications?.Select(item =>
            {
                var spec = GenericMapper<DeviceSpecification, DeviceSpecificationDTO>.MapTo(item);

                spec.DeviceSpecificationItemsList = item?.DeviceSpecificationsItems?
                        .Select(dSI => GenericMapper<DeviceSpecificationsItem, DeviceSpecificationItemDTO>.MapTo(dSI)).ToList();

                return spec;
            });

            return res;
        }
         
        public static IotsDevice MapToIotsDevice(CreateUpdateIotDeviceDTO source)
        {
            return iotSaveMapper.MappingTo(source);
        }
    }
}
