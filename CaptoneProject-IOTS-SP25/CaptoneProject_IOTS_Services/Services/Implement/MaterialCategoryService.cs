using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Business;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class MaterialCategoryService : IMaterialCategoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly MaterialCategoryRepository materialCategoryRepository;
        private readonly IFileService fileService;

        public MaterialCategoryService(
            IFileService fileService,
            MaterialCategoryRepository materialCategoryRepository
        )
        {
            _unitOfWork ??= new UnitOfWork();
            this.fileService = fileService;
            this.materialCategoryRepository = materialCategoryRepository;
        }

        public async Task<ResponseDTO> CreateOrUpdateMaterialCategory(int? id, CreateUpdateMaterialCategoryDTO payload)
        {
            MaterialCategory materialCategory = (id == null) ? new MaterialCategory() : materialCategoryRepository.GetById((int)id);

            if (materialCategory == null)
                return ResponseService<Object>.NotFound(ExceptionMessage.MATERIAL_CATEGORY_NOTFOUND);

            try
            {
                materialCategory.Label = payload.Label;
                materialCategory.ImageUrl = payload.ImageUrl;

                MaterialCategory? response;

                if (id > 0) //Update
                    response = materialCategoryRepository.Update(materialCategory);
                else //Create
                    response = materialCategoryRepository.Create(materialCategory);

                return await GetByMaterialCategoryId(response.Id);
            }
            catch (Exception ex)
            {
                return ResponseService<Object>.BadRequest(ex.Message);
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

        public async Task<ResponseDTO> GetAllMaterialCategory(string searchKeyword)
        {
            PaginationResponseDTO<MaterialCategory> res = materialCategoryRepository.GetPaginate(
                filter: m => m.Label.Contains(searchKeyword) && m.IsActive > 0,
                orderBy: null,
                includeProperties: "",
                pageIndex: 0,
                pageSize: 500000
            );

            return new ResponseDTO
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = res
            };
        }

        public async Task<GenericResponseDTO<MaterialCategory>> GetByMaterialCategoryId(int id)
        {
            var res = materialCategoryRepository.GetById(id);

            if (res == null)
                return ResponseService<MaterialCategory>.NotFound("Not Found");

            return ResponseService<MaterialCategory>.OK(res);
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<MaterialCategory>>> GetPaginationMaterialCategories(PaginationRequest paginate, int? statusFilter)
        {
            PaginationResponseDTO<MaterialCategory> res = materialCategoryRepository.GetPaginate(
                filter: m => m.Label.Contains(paginate.SearchKeyword)
                    && (statusFilter == null || m.IsActive == statusFilter),
                orderBy: orderBy => orderBy.OrderByDescending(item => item.Id),
                includeProperties: "",
                pageIndex: paginate.PageIndex,
                pageSize: paginate.PageSize
            );

            return ResponseService<PaginationResponseDTO<MaterialCategory>>.OK(res);
        }

        public async Task<ResponseDTO> UpdateMaterialCategoryStatus(int id, int isActive)
        {
            MaterialCategory res = materialCategoryRepository.GetById(id);

            if (res == null)
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = ExceptionMessage.MATERIAL_CATEGORY_NOTFOUND
                };

            res.IsActive = isActive;

            var response = materialCategoryRepository.Update(res);

            return await GetByMaterialCategoryId(response.Id);
        }
    }
}
