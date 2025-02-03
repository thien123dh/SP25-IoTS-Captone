using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IStoreService
    {
        Task<GenericResponseDTO<UserRequestResponseDTO>> CreateStoreUserRequestVerifyOtp(string email);
        Task<GenericResponseDTO<UserResponseDTO>> RegisterStoreUser(UserRegisterDTO payload);
        Task<GenericResponseDTO<StoreDetailsResponseDTO>> SubmitStoreInfomation(int userId, StoreRequestDTO payload);
        Task<GenericResponseDTO<StoreDetailsResponseDTO>> CreateOrUpdateStoreByUserId(int userId, StoreRequestDTO payload);
        Task<GenericResponseDTO<StoreDetailsResponseDTO>> GetStoreDetailsByUserId(int userId);
        Task<ResponseDTO> GetPaginationStores(PaginationRequest paginationRequest);
    }
}
