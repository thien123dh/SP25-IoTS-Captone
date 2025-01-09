using Azure;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IUserRequestService
    {
        Task<ResponseDTO> CreateOrUpdateUserRequest(string email, int userRequestStatus/*, string decision, string reason*/);
        Task<ResponseDTO> GetUserRequestPagination(
            PaginationRequest paginationRequest
        );
    }
}
