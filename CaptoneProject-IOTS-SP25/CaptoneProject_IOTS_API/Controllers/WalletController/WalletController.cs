using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.WalletController
{
    [Route("api/wallet")]
    [ApiController]
    public class WalletController : MyBaseController.MyBaseController
    {
        private readonly IWalletService walletService;

        public WalletController(IWalletService walletService)
        {
            this.walletService = walletService;
        }

        [HttpGet("get-wallet-by-user-id/{userId}")]
        public async Task<IActionResult> GetWalletByUserId(int userId)
        {
            var res = await walletService.GetWalletByUserId(userId);

            return GetActionResult(res);
        }
    }
}
