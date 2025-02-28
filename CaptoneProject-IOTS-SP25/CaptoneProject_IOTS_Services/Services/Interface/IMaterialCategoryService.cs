using Azure;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Business;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IMaterialCategoryService
    {
        Task<ResponseDTO> CreateOrUpdateMaterialCategory(int? id, CreateUpdateMaterialCategoryDTO categoryMaterial);

        Task<ResponseDTO> CreateOrUpdateMaterialCategoryToStore(int? id, CreateUpdateMaterialCategoryDTO payload);
        Task<ResponseDTO> UpdateMaterialCategoryStatus(int id, int IsActive);

        Task<ResponseDTO> DeleteMaterialCategory(int id);
        Task<ResponseDTO> GetAllMaterialCategory(string searchKeyword);
        Task<ResponseDTO> GetPaginationMaterialCategories(PaginationRequest paginate, int? statusFilter);
        Task<GenericResponseDTO<MaterialCategory>> GetByMaterialCategoryId(int id);
    }
}
