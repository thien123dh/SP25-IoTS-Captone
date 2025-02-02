using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Business;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class MaterialService : IMaterialService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly MaterialRepository materialRepository;
        private readonly IFileService fileService;

        public MaterialService(
            IFileService fileService,
            MaterialRepository materialRepository
        )
        {
            _unitOfWork ??= new UnitOfWork();
            this.fileService = fileService;
            this.materialRepository = materialRepository;
        }
        public async Task<ResponseDTO> CreateOrUpdateMaterial(int? id, CreateUpdateMaterialDTO material)
        {
            Material materials = (id == null) ? new Material() : materialRepository.GetById((int)id);

            if (materials == null && id != null)
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = ExceptionMessage.MATERIAL_NOTFOUND
                };

            try
            {
                materials.Description = material.Description;

                Material? response;

                if (id > 0) //Update
                    response = materialRepository.Update(materials);
                else //Create
                    response = materialRepository.Create(materials);

                return new ResponseDTO
                {
                    IsSuccess = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = await GetByMaterialId(response.Id)
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetAllMaterial(string searchKeyword)
        {
            PaginationResponseDTO<Material> res = materialRepository.GetPaginate(
               filter: m => m.Description.Contains(searchKeyword) && m.IsActive > 0,
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

        public async Task<GenericResponseDTO<Material>> GetByMaterialId(int id)
        {
            var res = materialRepository.GetById(id);

            if (res == null)
                return new GenericResponseDTO<Material>
                {
                    IsSuccess = false,
                    Message = "Not Found",
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };

            return new GenericResponseDTO<Material>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = res
            };
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<Material>>> GetPaginationMaterial(PaginationRequest paginate)
        {
            PaginationResponseDTO<Material> res = materialRepository.GetPaginate(
                filter: m => m.Description.Contains(paginate.SearchKeyword),
                orderBy: null,
                includeProperties: "",
                pageIndex: paginate.PageIndex,
                pageSize: paginate.PageSize
            );

            return new GenericResponseDTO<PaginationResponseDTO<Material>>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = res
            };
        }

        public async Task<ResponseDTO> UpdateMaterialStatus(int id, int IsActive)
        {
            Material res = materialRepository.GetById(id);

            if (res == null)
                return new ResponseDTO
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = ExceptionMessage.MATERIAL_NOTFOUND
                };

            res.IsActive = IsActive;

            var response = materialRepository.Update(res);

            return await GetByMaterialId(response.Id);
        }
    }
}
