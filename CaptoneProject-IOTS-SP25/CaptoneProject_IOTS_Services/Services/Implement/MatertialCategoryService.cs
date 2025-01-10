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

        public async Task<IBusinessResult> CreateMaterialCategory(MatertialCategoryRequestDTO categoryMaterial)
        {
            try
            {
                int result = await _unitOfWork.MaterialCategoryRepository.CreateAsync(new List<MatertialCategoryRequestDTO> { categoryMaterial });
                if (result > 0)
                {
                    return new BusinessResult(1, "success");
                }
                else
                {
                    return new BusinessResult(2, "fail");
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(-4, ex.Message);
            }
        }

        public Task<IBusinessResult> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IBusinessResult> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public Task<IBusinessResult> GetAllMaterialCategory(PaginationRequest paginationRequest)
        {
            throw new NotImplementedException();
        }

        public Task<IBusinessResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IBusinessResult> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IBusinessResult> UpdateAsync(MaterialCategory categoryMaterial)
        {
            throw new NotImplementedException();
        }
    }
}
