using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.TransactionDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.TransactionController
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : MyBaseController.MyBaseController
    {
        private readonly ITransactionService transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            this.transactionService = transactionService;
        }

        [HttpPost("get-transaction-pagination")]
        public async Task<IActionResult> GetTransactionPagination([FromBody] PaginationRequest payload)
        {
            var res = await transactionService.GetTransactionPagination(payload);

            return GetActionResult(res);
        }

    }
}
