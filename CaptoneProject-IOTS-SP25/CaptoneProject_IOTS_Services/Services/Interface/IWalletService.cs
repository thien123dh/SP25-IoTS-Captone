﻿using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IWalletService
    {
        Task<GenericResponseDTO<Wallet>> CreateOrUpdateWallet(CreateUpdateWalletDTO source);
        Task<GenericResponseDTO<Wallet>> GetWalletByUserId(int userId);
        Task<GenericResponseDTO<Wallet>> CreateTransactionUserWallet(CreateTransactionWalletDTO source);
        Task<bool> CheckWalletBallance(decimal fee, int userId);
        Task<ResponseDTO> UpdateUserWalletOrderTransactionAsync(List<UpdateUserWalletRequestDTO> request, string? orderCode = null, decimal? refundAmount = null);
    }
}
