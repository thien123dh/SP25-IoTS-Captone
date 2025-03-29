using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.TransactionDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.AdminController
{
    [Route("api/back-office")]
    [ApiController]
    public class BackofficeController : MyBaseController.MyBaseController
    {
        private readonly ITransactionService transactionService;
        private readonly IWalletService walletService;
        private readonly IUserServices userServices;

        public BackofficeController(
            ITransactionService transactionService,
            IWalletService walletService,
            IUserServices userServices
        )
        {
            this.transactionService = transactionService;
            this.walletService = walletService;
            this.userServices = userServices;
        }

        [HttpPost("send-currency-to-user")]
        public async Task<IActionResult> SendCurrencyToUser([FromBody] UpdateUserWalletRequestDTO payload)
        {
            var loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return Unauthorized();

            var source = new CreateTransactionWalletDTO
            {
                Amount = payload.Amount,
                UserId = payload.UserId,
                Description = "You received {Amount} gold from system".Replace("{Amount}", payload.Amount.ToString()),
                TransactionType = TransactionTypeEnum.SYSTEM_REWARD,
            };

            var res = await walletService.CreateTransactionUserWallet(source);

            if (res.IsSuccess && loginUserId != null)
            {
                await transactionService.CreateTransactionAsync(new CreateTransactionDTO
                {
                    Amount = payload.Amount,
                    Description = "You sent {Amount} to user {Email}"
                        .Replace("{Amount}", payload.Amount.ToString())
                        .Replace("{Email}", res?.Data?.UserNavigation.Email),
                    TransactionType = source.TransactionType,
                    Status = TransactionStatusEnum.SUCCESS,
                    UserId = (int)loginUserId,
                });
            }

            return GetActionResult(new CaptoneProject_IOTS_BOs.ResponseDTO
            {
                IsSuccess = res.IsSuccess,
                StatusCode = res.StatusCode,
                Message = res.IsSuccess ? "Success" : res.Message
            });
        }
    }
}
