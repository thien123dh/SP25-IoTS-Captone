using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.TransactionDTO;
using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface ITransactionService
    {
        //public Task<GenericResponseDTO<Transaction>> CreateTransactionWithinUpdateWallet(CreateTransactionDTO request);
        public Task<GenericResponseDTO<Transaction>> CreateTransactionAsync(CreateTransactionDTO request);
        public Task<GenericResponseDTO<PaginationResponseDTO<Transaction>>> GetTransactionPagination(PaginationRequest request);
    }
}
