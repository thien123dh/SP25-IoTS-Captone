using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.StoreDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.DTO.StoreDTO.StoreDTO;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface ITrainerService
    {
        Task<GenericResponseDTO<UserRequestResponseDTO>> CreateTrainerUserRequestVerifyOtp(string email);
        Task<GenericResponseDTO<UserResponseDTO>> RegisterTrainerUser(UserRegisterDTO payload);
        Task<GenericResponseDTO<TrainerBusinessLicense>> CreateOrUpdateTrainerBusinessLicences(TrainerBusinessLicensesDTO payload);
        Task<GenericResponseDTO<TrainerBusinessLicense>> GetTrainerBusinessLicenseByTrainerId(int trainerId);
    }
}
