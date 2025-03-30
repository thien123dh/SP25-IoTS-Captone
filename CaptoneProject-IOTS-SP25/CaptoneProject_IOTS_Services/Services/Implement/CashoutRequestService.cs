using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.CashoutRequestDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.EntityTypeConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class CashoutRequestService : ICashoutService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userServices;
        private readonly IWalletService walletService;
        private readonly int MAX_REQUEST_COUNT = 10;
        public CashoutRequestService(UnitOfWork unitOfWork, IUserServices userServices, IWalletService walletService)
        {
            this.unitOfWork = unitOfWork;
            this.userServices = userServices;
            this.walletService = walletService;
        }

        public async Task<GenericResponseDTO<CashoutRequest>> ApproveOrRejectCashoutRequest(int id, bool isApprove, RemarkDTO? remarks = null)
        {
            var cashoutRequest = unitOfWork.CashoutRequestRepository.GetById(id);
            var loginUserId = userServices.GetLoginUserId();
            var role = userServices.GetRole();

            var adminOrStaffRoles = new List<int?> { (int)RoleEnum.ADMIN, (int)RoleEnum.STAFF };

            if (!adminOrStaffRoles.Contains(role))
                return ResponseService<CashoutRequest>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            if (cashoutRequest.Status != (int)CashoutRequestStatusEnum.PENDING_TO_APPROVE)
                return ResponseService<CashoutRequest>.BadRequest("This request has been already handled. Please try again");

            Notifications notifications = null;

            Wallet? userWallet = null;

            try
            {
                if (isApprove)
                {
                    notifications = new Notifications
                    {
                        EntityId = id,
                        EntityType = (int)EntityTypeEnum.CASHOUT_REQUEST,
                        Content = $"Your cash out request has been approved. Please check your transaction history",
                        Title = $"Your cash out request has been approved. Please check your transaction history",
                        ReceiverId = cashoutRequest.CreatedBy
                    };

                    userWallet = (await walletService.GetWalletByUserId(cashoutRequest.CreatedBy))?.Data;

                    if ((userWallet?.Ballance ?? 0) < cashoutRequest.Amount)
                    {
                        return ResponseService<CashoutRequest>.BadRequest("Please reject this request. User Wallet Balance is insufficient");
                    }

                    cashoutRequest.Status = (int)CashoutRequestStatusEnum.APPROVED;
                    cashoutRequest.ActionDate = DateTime.Now;
                    cashoutRequest.ActionBy = (int)loginUserId;
                }
                else
                {
                    notifications = new Notifications
                    {
                        EntityId = id,
                        EntityType = (int)EntityTypeEnum.CASHOUT_REQUEST,
                        Content = $"Your cash out request has been rejected. Please go to cash out request history to check remarks",
                        Title = $"Your cash out request has been rejected. Please go to cash out request history to check remarks",
                        ReceiverId = cashoutRequest.CreatedBy
                    };

                    if (remarks != null)
                        cashoutRequest.Remarks = remarks.Remark;
                    cashoutRequest.Status = (int)CashoutRequestStatusEnum.REJECTED;
                    cashoutRequest.ActionDate = DateTime.Now;
                    cashoutRequest.ActionBy = (int)loginUserId;
                }

                if (notifications != null)
                    _ = unitOfWork.NotificationRepository.Create(notifications);
            } catch
            {

            }

            cashoutRequest = unitOfWork.CashoutRequestRepository.Update(cashoutRequest);

            if (userWallet != null)
            {
                userWallet.Ballance -= cashoutRequest.Amount;

                if (userWallet.Ballance < 0)
                    userWallet.Ballance = 0;

                _ = unitOfWork.WalletRepository.Update(userWallet);

                Transaction transaction = new Transaction
                {
                    Amount = cashoutRequest.Amount,
                    CreatedDate = DateTime.Now,
                    CurrentBallance = userWallet.Ballance,
                    TransactionType = "Cashout",
                    Status = "Success",
                    UserId = cashoutRequest.CreatedBy,
                    Description = $"You have cashed out {cashoutRequest.Amount} gold"
                };

                _ = unitOfWork.TransactionRepository.Create(transaction);
            }

            return ResponseService<CashoutRequest>.OK(cashoutRequest);
        }

        public CashoutRequest BuildToCashoutRequest(CashoutRequest target, CreateCashoutRequestDTO request)
        {
            var loginUserId = userServices.GetLoginUserId();
            var res = GenericMapper<CreateCashoutRequestDTO, CashoutRequest>.MapTo(request);

            res.CreatedBy = target.Id > 0 ? target.CreatedBy : (int)loginUserId;
            res.CreatedDate = target.Id > 0 ? target.CreatedDate : DateTime.Now;
            res.ActionDate = DateTime.Now;
            res.ActionBy = (int)loginUserId;

            return res;
        }

        public async Task<GenericResponseDTO<CashoutRequest>> CreateCashoutRequest(CreateCashoutRequestDTO request)
        {
            int loginUserId = (int)userServices.GetLoginUserId();

            var role = userServices.GetRole();

            if (role != (int)RoleEnum.TRAINER && role != (int)RoleEnum.STORE)
                return ResponseService<CashoutRequest>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var walletRes = await walletService.GetWalletByUserId(loginUserId);

            var wallet = walletRes?.Data;

            var countDateRequest = unitOfWork.CashoutRequestRepository.Search(
                item => item.CreatedBy == loginUserId && DateTime.Now.Date == item.CreatedDate.Date
            ).Count();

            if (countDateRequest > MAX_REQUEST_COUNT)
                return ResponseService<CashoutRequest>.BadRequest($"You are not allowed to create more then {MAX_REQUEST_COUNT} request per day");
            else if (request.Amount <= 0)
                return ResponseService<CashoutRequest>.BadRequest("Amount to cash out must be greater than 0");
            else if (wallet?.Ballance < request.Amount)
                return ResponseService<CashoutRequest>.BadRequest(ExceptionMessage.INSUFFICIENT_WALLET);

            try
            {
                var source = new CashoutRequest();
                source = BuildToCashoutRequest(source, request);

                source = unitOfWork.CashoutRequestRepository.Create(source);

                return ResponseService<CashoutRequest>.OK(source);
            } catch
            {
                return ResponseService<CashoutRequest>.NotFound("Cannot create cash out request. Please try again");
            }
        }

        public CashoutRequestResponseDTO BuildToCashoutRequestResponseDTO(CashoutRequest source)
        {
            var res = GenericMapper<CashoutRequest, CashoutRequestResponseDTO>.MapTo(source);

            return res;
        }

        public async Task<ResponseDTO> GetPaginationCashoutRequest(int? statusFilter, PaginationRequest request)
        {
            var loginUserId = userServices.GetLoginUserId();
            var role = userServices.GetRole();

            Expression<Func<CashoutRequest, bool>> func = item => true;

            if (role == (int)RoleEnum.ADMIN || role == (int)RoleEnum.STAFF)
                func = item => (statusFilter == null) || (statusFilter == item.Status);
            else if (role == (int)RoleEnum.STORE || role == (int)RoleEnum.TRAINER)
                func = item => item.CreatedBy == loginUserId && (statusFilter == null) || (statusFilter == item.Status);

            var pagination = unitOfWork.CashoutRequestRepository.GetPaginate(
                filter: func,
                orderBy: ob => ob.OrderByDescending(item => item.CreatedDate),
                includeProperties: "CreatedByNavigation,ActionByNavigation",
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            var res = PaginationMapper<CashoutRequest, CashoutRequestResponseDTO>.MapTo(BuildToCashoutRequestResponseDTO, pagination);

            return ResponseService<object>.OK(res);
        }
    }
}
