﻿using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using System.Security.Claims;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IUserServices
    {
        Task<ResponseDTO> LoginUserAsync(string email, string password);
        Task<ResponseDTO> GetUsersPagination(PaginationRequest paginationRequest, int? roleId);
        Task<ResponseDTO> GetAllUsers();
        Task<ResponseDTO> UpdateUserRole(int userId, List<int>? roleList);
        Task<GenericResponseDTO<UserResponseDTO>> UpdateUserStatus(int userId, int isActive);
        Task<ResponseDTO> UpdateUserPassword(int userId, string password);
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsById(int id);
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsByEmail(string email);
        Task<GenericResponseDTO<UserResponseDTO>> CreateOrUpdateUser(int id, CreateUserDTO payload, int isActive);
        int? GetLoginUser();
        Task<GenericResponseDTO<UserResponseDTO>> GetUserLoginInfo();
        Task<ResponseDTO> UserChangePassword(ChangePasswordRequestDTO payload);
    }
}
