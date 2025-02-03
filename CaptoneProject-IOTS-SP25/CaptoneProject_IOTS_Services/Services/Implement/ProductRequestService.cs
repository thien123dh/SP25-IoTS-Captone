using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.Constant.ProductRequestConst;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class ProductRequestService : IProductRequestService
    {
        private readonly ProductRequestRepository productRequestRepository;
        private readonly IUserServices userServices;
        private readonly IMaterialService materialService;

        public ProductRequestService (ProductRequestRepository productRequestRepository, 
            IUserServices userServices,
            IMaterialService materialService)
        {
            this.productRequestRepository = productRequestRepository;
            this.userServices = userServices;
            this.materialService = materialService;
        }

        public Task<GenericResponseDTO<ProductRequestDTO>> ApproveOrRejectProductRequest(int productRequestId, int isApprove)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<ProductRequestDTO>>> GetPaginationProductRequest(PaginationRequest payload)
        {
            List<Role>? loginUserRoles = await userServices.GetLoginUserRoles();

            if (loginUserRoles == null)
                return new GenericResponseDTO<PaginationResponseDTO<ProductRequestDTO>>
                {
                    IsSuccess = false,
                    Message = "You don't have permission to access, Please Login",
                    StatusCode = System.Net.HttpStatusCode.NonAuthoritativeInformation
                };

            var list = productRequestRepository.GetPaginate(
                filter: null,
                orderBy: ob => ob.OrderByDescending(item => item.UpdatedDate),
                "MaterialNavigation,MaterialGroupNavigation,LabNavigation,CreatedByNavigation",
                payload.PageIndex,
                payload.PageSize
            );

            return new GenericResponseDTO<PaginationResponseDTO<ProductRequestDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = PaginationMapper<ProductRequest, ProductRequestDTO>.MappingTo(ProductRequestMapper.MapToProductRequestDTO, list)
            };
        }

        public async Task<GenericResponseDTO<ProductRequestDTO>> GetProductRequestById(int id)
        {
            var res = await productRequestRepository.GetProductRequestById(id);

            if (res == null)
                return new GenericResponseDTO<ProductRequestDTO>
                {
                    IsSuccess = false,
                    Message = "Not Found",
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };

            return new GenericResponseDTO<ProductRequestDTO>
            {
                IsSuccess = true,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Data = ProductRequestMapper.MapToProductRequestDTO(res)
            };
        }

        public async Task<GenericResponseDTO<ProductRequestDTO>> SubmitMaterialRequest(int? productRequestId, CreateUpdateMaterialDTO payload)
        {
            var productRequest = (productRequestId == null) ? new ProductRequest() : await productRequestRepository.GetProductRequestById((int)productRequestId);

            int? loginUserId = userServices.GetLoginUserId();

            if (productRequest == null)
                return new GenericResponseDTO<ProductRequestDTO>
                {
                    IsSuccess = false,
                    Message = "Not Found",
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };

            var createProductResponse = await materialService.CreateOrUpdateMaterial(productRequest.MaterialId, payload);

            if (!createProductResponse.IsSuccess)
                return new GenericResponseDTO<ProductRequestDTO>
                    {
                        IsSuccess = false,
                        Message = createProductResponse.Message,
                        StatusCode = createProductResponse.StatusCode
                    };

            productRequest.ActionBy = loginUserId;
            productRequest.MaterialId = createProductResponse?.Data?.Id;
            productRequest.ProductId = createProductResponse?.Data?.Id;
            productRequest.ProductType = ((int)ProductTypeEnum.MATERIAL);
            productRequest.Status = (int)ProductRequestStatusEnum.PENDING_TO_APPROVED;
            productRequest.UpdatedBy = loginUserId;
            productRequest.UpdatedDate = DateTime.Now;

            try
            {

                if (productRequestId > 0) //Update
                {
                    productRequest = productRequestRepository.Update(productRequest);

                } else
                {
                    productRequest.CreatedBy = loginUserId;
                    productRequest.CreatedDate = DateTime.Now;

                    productRequest = productRequestRepository.Create(productRequest);
                }

            } catch (Exception ex)
            {
                return new GenericResponseDTO<ProductRequestDTO>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }

            return await GetProductRequestById(productRequest.Id);
        }
    }
}
