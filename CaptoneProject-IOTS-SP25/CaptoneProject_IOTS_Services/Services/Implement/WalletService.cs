using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
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

        public WalletService(
            WalletRepository walletRepository, 
            UserRepository userRepository
        )
        {
            this.walletRepository = walletRepository;
            this.userRepository = userRepository;

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
