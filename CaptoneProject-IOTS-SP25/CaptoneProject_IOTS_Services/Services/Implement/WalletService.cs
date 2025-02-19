using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.TransactionDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class WalletService : IWalletService
    {
        private readonly WalletRepository walletRepository;
        private readonly UserRepository userRepository;
        private readonly ITransactionService transactionService;

        public WalletService(
            WalletRepository walletRepository, 
            UserRepository userRepository,
            ITransactionService transactionService
        )
        {
            this.walletRepository = walletRepository;
            this.userRepository = userRepository;
            this.transactionService = transactionService;
        }

        public bool CheckWalletBalance(int userId, decimal? fee)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckWalletBallance(decimal fee, int userId)
        {
            var wallet = await GetWalletByUserId(userId);

            var newBallance = (wallet?.Data?.Ballance == null ? 0 : wallet.Data.Ballance) - fee;

            return newBallance < 0;
        }

        public async Task<GenericResponseDTO<Wallet>> CreateOrUpdateWallet(CreateUpdateWalletDTO source)
        {
            var user = userRepository.GetById(source.UserId);

            if (user == null)
            {
                return ResponseService<Wallet>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);
            }

            var wallet = walletRepository.GetByUserId(user.Id);

            wallet = wallet == null ? new Wallet() : wallet;

            //SET DATA
            wallet.Ballance = source.Ballance;
            wallet.UserId = source.UserId;
            wallet.UpdatedDate = DateTime.Now;
            //SET DATA

            try
            {
                if (wallet.Id > 0) //UPDATE
                {
                    wallet = walletRepository.Update(wallet);
                } else //CREATE
                {
                    wallet = walletRepository.Create(wallet);
                }
            } catch
            {
                ResponseService<Wallet>.BadRequest("Cannot Update Wallet. Please try again");
            }

            return ResponseService<Wallet>.OK(wallet);
        }

        public async Task<GenericResponseDTO<Wallet>> CreateTransactionUserWallet(CreateTransactionWalletDTO source)
        {
            var user = userRepository.GetById(source.UserId);

            if (user == null)
                return ResponseService<Wallet>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);

            var wallet = await GetWalletByUserId(source.UserId);

            if (wallet == null)
                return ResponseService<Wallet>.NotFound("The Wallet cannot be found");

            decimal newBallance = wallet?.Data?.Ballance == null ? 0 : wallet.Data.Ballance + source.Amount;

            if (newBallance >= 0)
            {
                try
                {
                    var saveWallet = walletRepository.GetById(wallet.Data.Id);

                    saveWallet.Ballance = newBallance;

                    wallet = await CreateOrUpdateWallet(new CreateUpdateWalletDTO
                    {
                        Ballance = newBallance,
                        UserId = user.Id
                    });

                    if (wallet.IsSuccess)
                    {
                        var transaction = new CreateTransactionDTO
                        {
                            Amount = source.Amount,
                            Description = source.Description,
                            TransactionType = source.TransactionType,
                            UserId = user.Id,
                            Status = TransactionStatusEnum.SUCCESS,
                        };

                        transactionService.CreateTransactionAsync(transaction);
                    }
                } catch
                {
                    var transaction = new CreateTransactionDTO
                    {
                        Amount = source.Amount,
                        Description = source.Description,
                        TransactionType = source.TransactionType,
                        UserId = source.UserId,
                        Status = TransactionStatusEnum.FAILED,
                    };

                    transactionService.CreateTransactionAsync(transaction);

                    return ResponseService<Wallet>.BadRequest("Transaction was Failed. Please try again");
                }
            } else
            {
                ResponseService<Wallet>.BadRequest(ExceptionMessage.INSUFFICIENT_WALLET);
            }

            return wallet;
        }

        public async Task<GenericResponseDTO<Wallet>> GetWalletByUserId(int userId)
        {
            var user = userRepository.GetById(userId);

            if (user == null)
            {
                return ResponseService<Wallet>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);
            }

            var wallet = walletRepository.GetByUserId(userId);
            try
            {
                if (wallet == null)
                {
                    wallet = walletRepository.Create(
                        new Wallet
                        {
                            Ballance = 0,
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            UserId = user.Id,
                        }
                    );
                }

            } catch
            {
                ResponseService<Wallet>.BadRequest("Cannot get wallet information. Please try again");
            }

            return ResponseService<Wallet>.OK(wallet);
        }
    }
}
