using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_Service.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IUserServices
    {
        Task<ResponseDTO> LoginUserAsync(string email, string password);
        Task<ResponseDTO> GetUsersPagination(PaginationRequest paginationRequest);
        Task<ResponseDTO> GetAllUsers();
        Task<ResponseDTO> UpdateUserRole(int userId, List<int>? roleList);
        Task<ResponseDTO> UpdateUserStatus(int userId, int isActive);
        Task<ResponseDTO> GetUserDetailsById(int id);
    }
}
