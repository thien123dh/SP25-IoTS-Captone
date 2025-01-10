using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IMaterialCategoryService
    {
        Task<IBusinessResult> GetAllMaterialCategory(PaginationRequest paginationRequest);
        Task<IBusinessResult> GetByIdAsync(string id);
        Task<IBusinessResult> UpdateAsync(MaterialCategory categoryMaterial);
        Task<IBusinessResult> CreateMaterialCategory(MatertialCategoryRequestDTO categoryMaterial);
        Task<IBusinessResult> DeleteAsync(int id);
        Task<IBusinessResult> GetAllCategories();
        Task<IBusinessResult> GetById(int id);
    }
}
