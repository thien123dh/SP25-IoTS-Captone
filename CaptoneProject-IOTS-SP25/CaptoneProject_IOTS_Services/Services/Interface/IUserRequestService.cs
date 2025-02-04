using Azure;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.UserRequestConstant;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IUserRequestService
    {
        Task<GenericResponseDTO<UserRequestResponseDTO>> CreateOrUpdateUserRequest(UserRequestRequestDTO payload);
        Task<ResponseDTO> GetUserRequestPagination(int? userRequestStatusFilter, PaginationRequest paginationRequest);
        Task<ResponseDTO> VerifyOTP(string email, string otp);
        Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> UpdateUserRequestStatus(int requestId, UserRequestStatusEnum status);
        Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> GetUserRequestDetailsById(int requestId);
        Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> GetUserRequestDetailsByUserId(int userId);
        Task<GenericResponseDTO<UserRequestDetailsResponseDTO>> ApproveOrRejectRequestStatus(int requestId, string? remark, int isApprove);
        Task<GenericResponseDTO<UserRequest>> DeleteUserRequestById(int id);
    }
}
