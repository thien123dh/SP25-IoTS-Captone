using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IMaterialService
    {
        Task<GenericResponseDTO<MaterialDetailsResponseDTO>> CreateOrUpdateMaterial(int? id, CreateUpdateMaterialDTO material);
        Task<ResponseDTO> UpdateMaterialStatus(int id, int IsActive);
        Task<ResponseDTO> GetAllMaterial(string searchKeyword);
        Task<GenericResponseDTO<PaginationResponseDTO<MaterialItemDTO>>> GetPaginationMaterial(PaginationRequest paginate);
        Task<GenericResponseDTO<MaterialDetailsResponseDTO>> GetByMaterialId(int id);
        Task<GenericResponseDTO<MaterialDetailsResponseDTO>> UpdateMaterialStatus(int id, ProductStatusEnum status);
    }
}
