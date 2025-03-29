using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
using System.Security.Claims;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IUserServices
    {
        Task<ResponseDTO> LoginUserAsync(string email, string password);
        Task<ResponseDTO> GetUsersPagination(PaginationRequest paginationRequest, int? roleId);
        Task<ResponseDTO> GetAllUsers();
        Task<GenericResponseDTO<UserDetailsResponseDTO>> UpdateUserRole(int userId, List<int>? roleList);
        Task<GenericResponseDTO<UserResponseDTO>> UpdateUserStatus(int userId, int isActive);
        Task<ResponseDTO> UpdateUserPassword(int userId, string password);
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsById(int id);
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsByEmail(string email);
        Task<GenericResponseDTO<UserResponseDTO>> CreateOrUpdateUser(int id, CreateUserDTO payload, int isActive);
        int? GetLoginUserId();
        User GetLoginUser();
        Task<List<Role>?> GetLoginUserRoles();
        Task<GenericResponseDTO<UserResponseDTO>> GetUserLoginInfo();
        Task<ResponseDTO> UserChangePassword(ChangePasswordRequestDTO payload);
        Task<bool> CheckLoginUserRole(RoleEnum role);
        Task<Boolean> CheckUserRole(int userId, RoleEnum role);

        Task<GenericResponseDTO<UserResponseDTO>> UpdateUserProfile(UpdateUserDTO request);

        Task<GenericResponseDTO<UserResponseDTO>> UpdateUserAvatar(UpdateUserAvatarDTO request);
        int? GetRole();
    }
}
