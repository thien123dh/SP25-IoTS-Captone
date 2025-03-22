using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.WarrantyRequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IWarrantyRequestService
    {
        Task<GenericResponseDTO<WarrantyRequestResponseDTO>> CreateOrUpdateWarrantyRequest(int? id, WarrantyRequestRequestDTO request);

        Task<GenericResponseDTO<WarrantyRequestResponseDTO>> GetWarrantyRequestById(int id);

        Task<GenericResponseDTO<PaginationResponseDTO<WarrantyRequestResponseDTO>>> GetWarrantyRequestPagination(PaginationRequest request);

        Task<GenericResponseDTO<WarrantyRequestResponseDTO>> UpdateWarrantyRequestStatus(int id, WarrantyRequestStatusEnum status);
    }
}
