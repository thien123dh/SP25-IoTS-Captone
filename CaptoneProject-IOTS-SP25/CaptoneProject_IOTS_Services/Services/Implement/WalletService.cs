using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.TransactionDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class WalletService : IWalletService
    {
        private readonly WalletRepository walletRepository;
        private readonly UserRepository userRepository;
        private readonly ITransactionService transactionService;
        private readonly UnitOfWork unitOfWork;

        public WalletService(
            WalletRepository walletRepository,
            UserRepository userRepository,
            ITransactionService transactionService,
            INotificationService notificationService,
            UnitOfWork unitOfWork
        )
        {
            this.walletRepository = walletRepository;
            this.userRepository = userRepository;
            this.transactionService = transactionService;
            this.unitOfWork = unitOfWork;
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

            decimal newBallance = wallet.Data?.Ballance == null ? 0 : (decimal)wallet.Data.Ballance + source.Amount;

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

        public async Task<ResponseDTO> UpdateUserWalletWithTransactionAsync(List<UpdateUserWalletRequestDTO> request)
        {
            try
            {
                var userIdsList = request.Select(item => item.UserId).ToList();

                var wallets = unitOfWork.WalletRepository.Search(
                    item => userIdsList.Any(id => id == item.UserId)
                ).ToList();

                if (wallets == null)
                    return ResponseService<object>.NotFound("No wallet can be found. Please try again");

                var notifications = request.Select(
                    item => new Notifications
                    {
                        EntityId = item.UserId,
                        EntityType = (int)EntityTypeConst.EntityTypeEnum.USER,
                        Title = $"You have received {item.Amount} gold from success orders",
                        Content = $"You have received {item.Amount} gold from success orders",
                        ReceiverId = item.UserId
                    }
                ).ToList();

                var transactions = request.Select(
                    item =>
                    {
                        var wallet = wallets.FirstOrDefault(w => w.UserId == item.UserId);

                        var transaction = new Transaction
                        {
                            UserId = item.UserId,
                            Amount = item.Amount,
                            CurrentBallance = wallet?.Ballance,
                            Description = $"You have received {item.Amount} gold",
                            TransactionType = TransactionTypeEnum.SUCCESS_ORDER,
                            Status = "Success"
                        };

                        return transaction;
                    }
                ).ToList();

                wallets = wallets?.Select(
                   wallet =>
                   {
                       var req = request.FirstOrDefault(r => r.UserId == wallet.UserId);

                       wallet.Ballance += (req?.Amount ?? 0);

                       return wallet;
                   }
                )?.ToList();

                if (notifications != null)
                    await unitOfWork.NotificationRepository.CreateAsync(notifications);

                if (transactions != null)
                    await unitOfWork.TransactionRepository.CreateAsync(transactions);

                if (wallets != null)
                    await unitOfWork.WalletRepository.UpdateAsync(wallets);

                return ResponseService<object>.OK(
                    request
                );
            } catch (Exception ex)
            {
                return ResponseService<object>.BadRequest(ex.Message);
            }
            
        }
    }
}
