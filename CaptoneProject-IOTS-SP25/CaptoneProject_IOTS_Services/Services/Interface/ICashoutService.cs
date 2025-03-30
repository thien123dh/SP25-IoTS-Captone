using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.CashoutRequestDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface ICashoutService
    {
        public Task<ResponseDTO> GetPaginationCashoutRequest(int? statusFilter, PaginationRequest request);

        public Task<GenericResponseDTO<CashoutRequest>> CreateCashoutRequest(CreateCashoutRequestDTO request);

        public Task<GenericResponseDTO<CashoutRequest>> ApproveOrRejectCashoutRequest(int id, bool isApprove, RemarkDTO? remarks = null);
    }
}
