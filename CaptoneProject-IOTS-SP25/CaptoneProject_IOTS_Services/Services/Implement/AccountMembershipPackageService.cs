﻿using CaptoneProject_IOTS_BOs;
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
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class AccountMembershipPackageService : IAccountMembershipPackageService
    {
        private readonly IUserServices userService;
        private readonly IWalletService walletService;
        private readonly UnitOfWork unitOfWork;

        public AccountMembershipPackageService(
            IWalletService walletService,
            UnitOfWork unitOfWork,
            IUserServices userServices
        )
        {
            this.walletService = walletService;
            this.unitOfWork = unitOfWork;
            this.userService = userServices;
        }
        public async Task<GenericResponseDTO<AccountMembershipPackage>> GetAccountMembershipPackageByUserId(int userId)
        {
            var user = unitOfWork.UserRepository.GetById(userId);

            if (user == null)
                return ResponseService<AccountMembershipPackage>.BadRequest(ExceptionMessage.EMAIL_DOESNT_EXIST);

            var package = unitOfWork.AccountMembershipPackageRepository.GetByUserId(userId);

            if (package == null)
            {
                return ResponseService<AccountMembershipPackage>.NotFound("You haven't registered any membership package. Please select one to be a member");
            }

            return ResponseService<AccountMembershipPackage>.OK(package);
        }

        public async Task<ResponseDTO> GetAllMembershipPackage()
        {
            int? packageFor = await userService.CheckLoginUserRole(RoleEnum.STORE) ? 
                (int)MembershipPackageTypeEnum.STORE : 
                await userService.CheckLoginUserRole(RoleEnum.TRAINER) ? (int)MembershipPackageTypeEnum.TRAINER : null;

            if (packageFor == null && !(await userService.CheckLoginUserRole(RoleEnum.ADMIN) || await userService.CheckLoginUserRole(RoleEnum.STAFF)))
                return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            return ResponseService<object>.OK(
                unitOfWork.MembershipPackageRepository.GetAll().Where(
                    item => ((packageFor == null) || item.PackageFor == packageFor)
                        && 
                        item.IsActive > 0
                ).ToList()
            );
        }

        public MembershipPackage? GetMembershipPackageById(int id)
        {
            return unitOfWork.MembershipPackageRepository.GetById(id);
        }

        public async Task<GenericResponseDTO<AccountMembershipPackage>> CreateOrUpdateAccountMembershipPackage(AccountRegisterMembershipPackageDTO request)
        {
            var user = unitOfWork.UserRepository.GetUserById(request.UserId);

            if (user == null)
                return ResponseService<AccountMembershipPackage>.BadRequest(ExceptionMessage.EMAIL_DOESNT_EXIST);

            var packageOption = GetMembershipPackageById(request.MembershipPackageId);

            if (packageOption == null)
                return ResponseService<AccountMembershipPackage>.BadRequest("Package option cannot be found");

            int userId = request.UserId;

            var package = unitOfWork.AccountMembershipPackageRepository.GetByUserId(userId);

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
                    package = unitOfWork.AccountMembershipPackageRepository.Update(package);
                } else
                {
                    package = unitOfWork.AccountMembershipPackageRepository.Create(package);
                }
            } catch
            {
                return ResponseService<AccountMembershipPackage>.BadRequest("Cannot Register Package. Please try again");
            }

            return ResponseService<AccountMembershipPackage>.OK(package);
        }

        public async Task<GenericResponseDTO<AccountMembershipPackage>> RegisterAccountMembershipPackage(AccountRegisterMembershipPackageDTO request)
        {
            var user = unitOfWork.UserRepository.GetById(request.UserId);

            if (user == null)
            {
                return ResponseService<AccountMembershipPackage>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);
            }

            var package = GetMembershipPackageById(request.MembershipPackageId);

            if (package == null)
                return ResponseService<AccountMembershipPackage>.NotFound("Membership Package Not Found");

            var accountMembershipPackage = unitOfWork.AccountMembershipPackageRepository.GetByUserId(request.UserId);

            accountMembershipPackage = accountMembershipPackage == null ? new AccountMembershipPackage() : accountMembershipPackage;

            //If user register package days < 5 days
            if (accountMembershipPackage.NextPaymentDate.Subtract(DateTime.Now).TotalDays > 5)
                return ResponseService<AccountMembershipPackage>.BadRequest(ExceptionMessage.REGISTER_PACKAGE_TOO_SOON);

            DateTime nextPayment = (DateTime.Now > accountMembershipPackage.NextPaymentDate) ? DateTime.Now : accountMembershipPackage.NextPaymentDate;
            
            int numberOfMonth = package.NumberOfMonth;
            accountMembershipPackage.Fee = package.Fee;
            accountMembershipPackage.NextPaymentDate = nextPayment.AddMonths(numberOfMonth);
            accountMembershipPackage.LastPaymentDate = DateTime.Now;
            accountMembershipPackage.UserId = user.Id;
            accountMembershipPackage.MembershipPackageType = package.Id;

            try
            {
                if (await walletService.CheckWalletBallance(package.Fee, user.Id))
                    return ResponseService<AccountMembershipPackage>.BadRequest(ExceptionMessage.INSUFFICIENT_WALLET);

                if (accountMembershipPackage.Id > 0)
                    accountMembershipPackage = unitOfWork.AccountMembershipPackageRepository.Update(accountMembershipPackage);
                else
                    accountMembershipPackage = unitOfWork.AccountMembershipPackageRepository.Create(accountMembershipPackage);

                //Update user status ==> 1
                await userService.UpdateUserStatus(user.Id, (int)UserStatusEnum.ACTIVE);

                await walletService.CreateTransactionUserWallet(new CreateTransactionWalletDTO
                {
                    Amount = -accountMembershipPackage.Fee,
                    Description = $"You have been charged {accountMembershipPackage.Fee} gold",
                    TransactionType = "Register Membership Package",
                    UserId = user.Id,
                });

                Transaction appTrans = new Transaction
                {
                    Amount = accountMembershipPackage.Fee,
                    CreatedDate = DateTime.Now,
                    CurrentBallance = 0,
                    Description = $"You have received {accountMembershipPackage.Fee} gold for Store/Trainer {user.Id} Registered",
                    Status = "Success",
                    TransactionType = "Register Membership Package",
                    UserId = AdminConst.ADMIN_ID,
                    IsApplication = 1
                };

                _ = unitOfWork.TransactionRepository.Create(appTrans);

            } catch
            {
                return ResponseService<AccountMembershipPackage>.BadRequest("Cannot register membership package. Please try again");
            }

            return ResponseService<AccountMembershipPackage>.OK(accountMembershipPackage);
        }
    }
}
