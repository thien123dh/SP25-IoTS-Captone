using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Business;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class MatertialCategoryService : IMaterialCategoryService
    {
        private readonly UnitOfWork _unitOfWork;

        public MatertialCategoryService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> CreateMaterialCategory(MatertialCategoryRequestDTO category)
        {
            try
            {
                var materialCategory = new MaterialCategory
                {
                    Label = category.Label,
                    IsActive = category.IsActive
                };
                _unitOfWork.MaterialCategoryRepository.Create(materialCategory);
                return new BusinessResult(1, "Create Category Material success");
            }
            catch (Exception ex)
            {
                return new BusinessResult(-4, ex.Message);
            }
        }

        public Task<IBusinessResult> DeleteMaterialCategoryAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IBusinessResult> GetAllMaterialCategory()
        {
            return new BusinessResult(1, "Get all category success", await _unitOfWork.MaterialCategoryRepository.GetAllMaterialCaterial());
        }

        public async Task<IBusinessResult> GetByMaterialCategoryId(int id)
        {
            var category = await _unitOfWork.MaterialCategoryRepository.GetCategoryMaterialById(id);
            if (category == null) return new BusinessResult(4, "category not found");
            return new BusinessResult(1, "Get Category success", category);
        }

        public Task<IBusinessResult> UpdateMaterialCategoryAsync(MaterialCategoryResponeDTO categoryMaterial)
        {
            throw new NotImplementedException();
        }
    }
}
