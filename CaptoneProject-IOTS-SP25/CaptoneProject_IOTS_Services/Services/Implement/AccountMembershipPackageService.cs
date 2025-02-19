using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MembershipPackageDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class AccountMembershipPackageService : IAccountMembershipPackageService
    {
        private readonly AccountMembershipPackageRepository accountMembershipPackageRepository;
        private readonly MembershipPackageRepository membershipPackageRepository;
        private readonly UserRepository userRepository;
        private readonly IWalletService walletService;

        public AccountMembershipPackageService (
            AccountMembershipPackageRepository accountMembershipPackageRepository,
            MembershipPackageRepository membershipPackageRepository,
            UserRepository userRepository,
            IWalletService walletService
        )
        {
            this.accountMembershipPackageRepository = accountMembershipPackageRepository;
            this.membershipPackageRepository = membershipPackageRepository;
            this.userRepository = userRepository;
            this.walletService = walletService;
        }
        public async Task<GenericResponseDTO<AccountMembershipPackage>> GetAccountMembershipPackageByUserId(int userId)
        {
            var user = userRepository.GetById(userId);

            if (user == null)
                return ResponseService<AccountMembershipPackage>.BadRequest(ExceptionMessage.EMAIL_DOESNT_EXIST);

            var package = accountMembershipPackageRepository.GetByUserId(userId);

            if (package == null)
            {
                return ResponseService<AccountMembershipPackage>.NotFound("You haven't registered any membership package. Please select one to be a member");
            }

            return ResponseService<AccountMembershipPackage>.OK(package);
        }

        public List<MembershipPackage> GetAllMembershipPackage()
        {
            return membershipPackageRepository.GetAll();
        }

        public MembershipPackage? GetMembershipPackageById(int id)
        {
            return membershipPackageRepository.GetById(id);
        }

        public async Task<GenericResponseDTO<AccountMembershipPackage>> CreateOrUpdateAccountMembershipPackage(AccountRegisterMembershipPackageDTO request)
        {
            var user = userRepository.GetUserById(request.UserId);

            if (user == null)
                return ResponseService<AccountMembershipPackage>.BadRequest(ExceptionMessage.EMAIL_DOESNT_EXIST);

            var packageOption = GetMembershipPackageById(request.MembershipPackageId);

            if (packageOption == null)
                return ResponseService<AccountMembershipPackage>.BadRequest("Package option cannot be found");

            int userId = request.UserId;

            var package = accountMembershipPackageRepository.GetByUserId(userId);

            package = package == null ? new AccountMembershipPackage() : package;

            //SET DATA
            var lastPaymentDate = package.LastPaymentDate;

            package.LastPaymentDate = DateTime.Now;
            package.NextPaymentDate = DateTime.Now.AddMonths(packageOption.NumberOfMonth);
            package.UserId = userId;
            package.MembershipPackageType = packageOption.Id;
            //SET DATA

            try
            {
                if (package.Id > 0) //UPDATE
                {
                    package = accountMembershipPackageRepository.Update(package);
                } else
                {
                    package = accountMembershipPackageRepository.Create(package);
                }
            } catch
            {
                return ResponseService<AccountMembershipPackage>.BadRequest("Cannot Register Package. Please try again");
            }

            return ResponseService<AccountMembershipPackage>.OK(package);
        }

        public async Task<GenericResponseDTO<AccountMembershipPackage>> RegisterAccountMembershipPackage(AccountRegisterMembershipPackageDTO request)
        {
            var user = userRepository.GetById(request.UserId);

            if (user == null)
            {
                return ResponseService<AccountMembershipPackage>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);
            }

            var package = GetMembershipPackageById(request.MembershipPackageId);

            if (package == null)
                return ResponseService<AccountMembershipPackage>.NotFound("Membership Package Not Found");

            var accountMembershipPackage = accountMembershipPackageRepository.GetByUserId(request.UserId);

            accountMembershipPackage = accountMembershipPackage == null ? new AccountMembershipPackage() : accountMembershipPackage;

            accountMembershipPackage.Fee = package.Fee;
            accountMembershipPackage.LastPaymentDate = DateTime.Now;
            accountMembershipPackage.NextPaymentDate = DateTime.Now;
            accountMembershipPackage.UserId = user.Id;
            accountMembershipPackage.MembershipPackageType = package.Id;

            try
            {
                if (await walletService.CheckWalletBallance(package.Fee, user.Id))
                    return ResponseService<AccountMembershipPackage>.BadRequest(ExceptionMessage.INSUFFICIENT_WALLET);

                if (accountMembershipPackage.Id > 0)
                    accountMembershipPackage = accountMembershipPackageRepository.Update(accountMembershipPackage);
                else
                    accountMembershipPackage = accountMembershipPackageRepository.Create(accountMembershipPackage);

                walletService.CreateTransactionUserWallet(new CreateTransactionWalletDTO
                {
                    Amount = -accountMembershipPackage.Fee,
                    Description = $"You have been charged {accountMembershipPackage.Fee} gold",
                    TransactionType = "Register Membership Package",
                    UserId = user.Id,
                });
            } catch
            {
                return ResponseService<AccountMembershipPackage>.BadRequest("Cannot register membership package. Please try again");
            }

            return ResponseService<AccountMembershipPackage>.OK(accountMembershipPackage);
        }
    }
}
