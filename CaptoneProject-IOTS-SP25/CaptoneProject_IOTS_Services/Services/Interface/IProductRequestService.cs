using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductRequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IProductRequestService
    {
        public Task<GenericResponseDTO<PaginationResponseDTO<ProductRequestDTO>>> GetPaginationProductRequest(PaginationRequest payload);
        public Task<GenericResponseDTO<ProductRequestDTO>> GetProductRequestById(int id);
        public Task<GenericResponseDTO<ProductRequestDTO>> SubmitMaterialRequest(int? productRequestId, CreateUpdateMaterialDTO payload);
        public Task<GenericResponseDTO<ProductRequestDTO>> ApproveOrRejectProductRequest(int productRequestId, int isApprove);

    }
}
