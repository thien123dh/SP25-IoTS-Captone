using Azure;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IUserRequestService
    {
        Task<GenericResponseDTO<UserRequestResponseDTO>> CreateOrUpdateUserRequest(UserRequestRequestDTO payload);
        Task<ResponseDTO> GetUserRequestPagination(int? userRequestStatusFilter, PaginationRequest paginationRequest);
        Task<ResponseDTO> VerifyOTP(string email, string otp);
        Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> GetUserRequestDetailsById(int requestId);
        Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> GetUserRequestDetailsByUserId(int userId);
        Task<ResponseDTO> ApproveOrRejectRequestStatus(int requestId, string? remark, int isApprove);
    }
}
