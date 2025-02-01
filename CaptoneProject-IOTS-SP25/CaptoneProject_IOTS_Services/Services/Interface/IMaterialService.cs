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

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IMaterialService
    {
        Task<ResponseDTO> CreateOrUpdateMaterial(int? id, MaterialRequestDTO material);
        Task<ResponseDTO> UpdateMaterialStatus(int id, int IsActive);
        Task<ResponseDTO> GetAllMaterial(string searchKeyword);
        Task<GenericResponseDTO<PaginationResponseDTO<Material>>> GetPaginationMaterial(PaginationRequest paginate);
        Task<GenericResponseDTO<Material>> GetByMaterialId(int id);
    }
}
