using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IIotDevicesService
    {
        public Task<GenericResponseDTO<IotDeviceDetailsDTO>> GetIotDeviceById(int id);
        public Task<GenericResponseDTO<IotDeviceDetailsDTO>> CreateOrUpdateIotDevice(int? id, CreateUpdateIotDeviceDTO payload);
        public Task<GenericResponseDTO<PaginationResponseDTO<IotDeviceItem>>> GetPagination(int? storeId, PaginationRequest payload);
        public Task<GenericResponseDTO<IotDeviceDetailsDTO>> UpdateIotDeviceStatus(int id, int status);
    }
}
