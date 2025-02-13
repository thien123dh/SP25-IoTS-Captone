using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialGroupCategoryDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Business;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class MaterialGroupCategoryService : IMaterialGroupCategoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly MaterialGroupCategoryRepository materialGroupCategoryRepository;
        private readonly IFileService fileService;

        public MaterialGroupCategoryService(
            IFileService fileService,
            MaterialGroupCategoryRepository materialGroupCategoryRepository
        )
        {
            _unitOfWork ??= new UnitOfWork();
            this.fileService = fileService;
            this.materialGroupCategoryRepository = materialGroupCategoryRepository;
        }

        public async Task<ResponseDTO> CreateOrUpdateMaterialGroupCategory(int? id, CreateUpdateMaterialGroupCategoryDTO categoryGroupMaterial)
        {
            MaterialGroupCategory materialGroupCategory = (id == null) ? new MaterialGroupCategory() : materialGroupCategoryRepository.GetById((int)id);

            if (materialGroupCategory == null)
                return ResponseService<Object>.NotFound(ExceptionMessage.MATERIAL_GROUP_CATEGORY_NOTFOUND);

            try
            {
                materialGroupCategory.Label = categoryGroupMaterial.Label;


                MaterialGroupCategory? response;

                if (id > 0) //Update
                {

                    response = materialGroupCategoryRepository.Update(materialGroupCategory);
                }
                else //Create
                    response = materialGroupCategoryRepository.Create(materialGroupCategory);

                return await GetByMaterialGroupCategoryId(response.Id);
            }
            catch (Exception ex)
            {
                return ResponseService<Object>.BadRequest(ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAllMaterialGroupCategory(string searchKeyword)
        {
            PaginationResponseDTO<MaterialGroupCategory> res = materialGroupCategoryRepository.GetPaginate(
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

        public async Task<GenericResponseDTO<MaterialGroupCategory>> GetByMaterialGroupCategoryId(int id)
        {
            var res = materialGroupCategoryRepository.GetById(id);

            if (res == null)
                return ResponseService<MaterialGroupCategory>.NotFound("Not Found");

            return ResponseService<MaterialGroupCategory>.OK(res);
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<MaterialGroupCategory>>> GetPaginationMaterialGroupCategories(PaginationRequest paginate, int? statusFilter)
        {
            PaginationResponseDTO<MaterialGroupCategory> res = materialGroupCategoryRepository.GetPaginate(
                filter: m => m.Label.Contains(paginate.SearchKeyword)
                    && (statusFilter == null || m.IsActive == statusFilter),
                orderBy: orderBy => orderBy.OrderByDescending(item => item.Id),
                includeProperties: "",
                pageIndex: paginate.PageIndex,
                pageSize: paginate.PageSize
            );

            return ResponseService<PaginationResponseDTO<MaterialGroupCategory>>.OK(res);
        }

        public async Task<ResponseDTO> UpdateMaterialGroupCategoryStatus(int id, int IsActive)
        {
            MaterialGroupCategory res = materialGroupCategoryRepository.GetById(id);

            if (res == null)
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = ExceptionMessage.MATERIAL_GROUP_CATEGORY_NOTFOUND
                };

            res.IsActive = IsActive;

            var response = materialGroupCategoryRepository.Update(res);

            return await GetByMaterialGroupCategoryId(response.Id);
        }
    }
}
