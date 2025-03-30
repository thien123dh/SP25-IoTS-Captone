using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IRefundRequestService
    {
        public Task<ResponseDTO> GetPaginationRefundRequest(int? statusFilter, PaginationRequest request);

        public Task<ResponseDTO> UpdateStatusToHandled(int requestId);
    }
}
