using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.DTO.MaterialGroupCategoryDTO;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IMaterialGroupCategoryService
    {
        Task<ResponseDTO> CreateOrUpdateMaterialGroupCategory(int? id, CreateUpdateMaterialGroupCategoryDTO categoryGroupMaterial);
        Task<ResponseDTO> UpdateMaterialGroupCategoryStatus(int id, int IsActive);
        Task<ResponseDTO> GetAllMaterialGroupCategory(string searchKeyword);
        Task<GenericResponseDTO<PaginationResponseDTO<MaterialGroupCategory>>> GetPaginationMaterialGroupCategories(PaginationRequest paginate, int? statusFilter);
        Task<GenericResponseDTO<MaterialGroupCategory>> GetByMaterialGroupCategoryId(int id);
    }
}
