using CaptoneProject_IOTS_BOs;
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
        Task<GenericResponseDTO<UserDetailsResponseDTO>> UpdateUserStatus(int userId, int isActive);
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsById(int id);
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsByEmail(string email);
        //Task<GenericResponseDTO<UserDetailsResponseDTO>> CreateOrUpdateUser(int id, UserCreateOrUpdateRequestDTO payload);
        Task<ResponseDTO> CreateStaffOrManager(CreateUserDTO payload);
        Task<ResponseDTO> StaffManagerVerifyOTP(string otp, int requestId, int requestStatusId, string password);
        int? GetLoginUser();
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserLoginInfo();
        Task<ResponseDTO> RegisterUser(UserRegisterDTO payload);
        Task<ResponseDTO> UserChangePassword(ChangePasswordRequestDTO payload);
    }
}
