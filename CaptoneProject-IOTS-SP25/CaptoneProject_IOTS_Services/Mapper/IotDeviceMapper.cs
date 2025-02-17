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
        private static readonly IMapService<IotsDevice, IotDeviceDetailsDTO> iotDetailMapper = new MapService<IotsDevice, IotDeviceDetailsDTO>();
        private static readonly IMapService<CreateUpdateIotDeviceDTO, IotsDevice> iotSaveMapper = new MapService<CreateUpdateIotDeviceDTO, IotsDevice>();

        public static IotDeviceDetailsDTO MapToIotDeviceDetailsDTO(IotsDevice source)
        {
            return iotDetailMapper.MappingTo(source);
        }
         
        public static IotsDevice MapToIotsDevice(CreateUpdateIotDeviceDTO source)
        {
            return iotSaveMapper.MappingTo(source);
        }
    }
}
