using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Business;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class MaterialService : IMaterialService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly MaterialRepository materialRepository;
        private readonly IFileService fileService;
        private readonly AttachmentRepository attachmentRepository;
        private readonly IAttachmentsService attachmentsService;
        private readonly IUserServices userServices;
        private readonly IStoreService storeService;
        public MaterialService(
            IFileService fileService,
            MaterialRepository materialRepository,
            AttachmentRepository attachmentRepository,
            IAttachmentsService attachmentsService,
            IUserServices userServices,
            IStoreService storeService
        )
        {
            _unitOfWork ??= new UnitOfWork();
            this.fileService = fileService;
            this.materialRepository = materialRepository;
            this.attachmentRepository = attachmentRepository;
            this.attachmentsService = attachmentsService;
            this.userServices = userServices;
            this.storeService = storeService;
        }
        public async Task<GenericResponseDTO<MaterialDetailsResponseDTO>> CreateOrUpdateMaterial(int? id, 
            CreateUpdateMaterialDTO payload)
        {
            Material source = (id == null) ? new Material() : materialRepository.GetById((int)id);

            int? loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return new GenericResponseDTO<MaterialDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Please Login as Store to Create Material",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

            var storeInformation = storeService.GetStoreDetailsByUserId((int)loginUserId);

            if (storeInformation == null)
                return new GenericResponseDTO<MaterialDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Your account don't have role Store. You are not allow to create or update the product",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

            if (source == null)
                return new GenericResponseDTO<MaterialDetailsResponseDTO>
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = ExceptionMessage.MATERIAL_NOTFOUND
                };

            try
            {
                var material = MaterialMapper.MapToMaterial(payload);

                material.Id = id == null ? material.Id : (int)id;
                material.CreatedBy = source.CreatedBy == null ? loginUserId : source.CreatedBy;
                material.CreatedDate = source.CreatedDate;
                material.UpdatedDate = DateTime.Now;
                material.UpdatedBy = loginUserId;
                material.IsActive = source.IsActive;
                material.StoreId = source.StoreId == 0 ? storeInformation.Id : source.StoreId;

                if (id > 0) //Update
                    material = materialRepository.Update(material);
                else //Create
                    material = materialRepository.Create(material);

                //Create or update material attachment
                var response = await attachmentsService.CreateOrUpdateAttachments(material.Id, (int)EntityTypeEnum.MATERIAL, payload.MaterialAttachments);

                if (!response.IsSuccess)
                    throw new Exception(response.Message);

                return await GetByMaterialId(material.Id);
            }
            catch (Exception ex)
            {
                return new GenericResponseDTO<MaterialDetailsResponseDTO>
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

        public async Task<GenericResponseDTO<MaterialDetailsResponseDTO>> GetByMaterialId(int id)
        {
            var res = materialRepository.GetById(id);

            if (res == null)
                return new GenericResponseDTO<MaterialDetailsResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Not Found",
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };

            List<Attachment>? attachments = attachmentRepository.GetAttachmentsByEntityId(id, (int)EntityTypeEnum.MATERIAL);

            MaterialDetailsResponseDTO result = MaterialMapper.MapToMaterialDetailsResponseDTO(res, attachments);

            return new GenericResponseDTO<MaterialDetailsResponseDTO>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = result
            };
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<MaterialItemDTO>>> GetPaginationMaterial(PaginationRequest paginate)
        {
            PaginationResponseDTO<Material> res = materialRepository.GetPaginate(
                filter: m => m.Name.Contains(paginate.SearchKeyword),
                orderBy: orderBy => orderBy.OrderByDescending(item => item.CreatedDate),
                includeProperties: "StoreNavigation,Category",
                pageIndex: paginate.PageIndex,
                pageSize: paginate.PageSize
            );

            return new GenericResponseDTO<PaginationResponseDTO<MaterialItemDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = PaginationMapper<Material, MaterialItemDTO>.MappingTo(MaterialMapper.MapToMaterialItemDTO, res)
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
