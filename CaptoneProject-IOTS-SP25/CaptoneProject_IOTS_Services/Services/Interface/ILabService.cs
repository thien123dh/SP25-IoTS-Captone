using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using Microsoft.Identity.Client;
using System.Linq.Expressions;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface ILabService
    {
        Task<ResponseDTO> GetComboLabsPagination(int comboId, PaginationRequest paginationRequest);
        Task<ResponseDTO> GetLabPagination(LabFilterRequestDTO filterRequest,
                            PaginationRequest paginationRequest,
                            Expression<Func<Lab, bool>>? additionalFunc = null);
        Task<ResponseDTO> GetCustomerManagementLabsPagination(PaginationRequest paginationRequest);
        Task<ResponseDTO> GetTrainerManagementLabsPagination(LabFilterRequestDTO filterRequest, PaginationRequest paginationRequest);

        Task<ResponseDTO> GetStoreManagementLabsPagination(int? comboId, PaginationRequest paginationRequest);

        Task<LabDetailsInformationResponseDTO> GetLabDetailsInformation(int labId);

        Task<GenericResponseDTO<LabDetailsInformationResponseDTO>> CreateOrUpdateLabDetailsInformation(int? labId, CreateUpdateLabInformationDTO request);

        Task<GenericResponseDTO<List<LabVideoResponseDTO>>> GetLabVideoList(int labId);

        Task<GenericResponseDTO<List<LabVideoResponseDTO>>> CreateOrUpdateLabVideoList(int labId, List<CreateUpdateLabVideo> requestList);
        Task<GenericResponseDTO<LabDetailsInformationResponseDTO>> ApproveOrRejectLab(int labId, bool isApprove, RemarkDTO? payload = null);
        Task<GenericResponseDTO<LabDetailsInformationResponseDTO>> SubmitLabRequest(int labId);
        Task<bool> CheckPermissionToViewLabVideoList(int labId);
        Task<ResponseDTO> ActiveOrDeactiveLab(int labId, bool isDeactive = false);
    }
}
