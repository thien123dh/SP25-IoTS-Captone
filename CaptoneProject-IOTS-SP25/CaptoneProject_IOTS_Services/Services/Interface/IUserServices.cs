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
        Task<ResponseDTO> UpdateUserStatus(int userId, int isActive);
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserDetailsById(int id);
        //Task<GenericResponseDTO<UserDetailsResponseDTO>> CreateOrUpdateUser(int id, UserCreateOrUpdateRequestDTO payload);
        Task<ResponseDTO> CreateStaffOrManager(UserDetailsRequestDTO payload);
        Task<ResponseDTO> StaffManagerVerifyOTP(string otp, int requestId, int requestStatusId, string password);
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetLoginUser();
        Task<GenericResponseDTO<UserDetailsResponseDTO>> GetUserLoginInfo(ClaimsPrincipal user);
        Task<ResponseDTO> RegisterUser(UserRegisterDTO payload);
    }
}
