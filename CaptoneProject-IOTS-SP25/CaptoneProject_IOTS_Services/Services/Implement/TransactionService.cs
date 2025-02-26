using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.TransactionDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;

namespace CaptoneProject_IOTS_Service.Services.Implement
{

    public class TransactionService : ITransactionService
    {
        private readonly TransactionRepository transactionRepository;
        private readonly IUserServices userServices;

        public TransactionService(TransactionRepository transactionRepository,
            IUserServices userService)
        {
            this.transactionRepository = transactionRepository;
            this.userServices = userService;
        }

        public async Task<GenericResponseDTO<Transaction>> CreateTransactionAsync(CreateTransactionDTO request)
        {
            var user = userServices.GetUserDetailsById(request.UserId);

            if (user == null)
                return ResponseService<Transaction>.NotFound(ExceptionMessage.USER_DOESNT_EXIST);

            var transaction = new Transaction
            {
                Amount = request.Amount,
                CreatedDate = DateTime.Now,
                Description = request.Description,
                TransactionType = request.TransactionType,
                UserId = request.UserId,
                Status = request.Status,
            };

            try
            {
                transaction = transactionRepository.Create(transaction);
            } catch
            {
                return ResponseService<Transaction>.BadRequest("Cannot create transaction history");
            }

            return ResponseService<Transaction>.OK(transaction);
        }

        public async Task<GenericResponseDTO<PaginationResponseDTO<Transaction>>> GetTransactionPagination(PaginationRequest request)
        {
            var loginUserId = userServices.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<PaginationResponseDTO<Transaction>>.Unauthorize(ExceptionMessage.INVALID_PERMISSION);

            var pagination = transactionRepository.GetPaginate(
                filter: item => item.UserId == loginUserId && 
                    (request.SearchKeyword == null || item.Description.Contains(request.SearchKeyword) || item.TransactionType.Contains(request.SearchKeyword))
                    &&
                    (request.StartFilterDate == null) || (request.StartFilterDate <= item.CreatedDate)
                    &&
                    (request.EndFilterDate == null) || (item.CreatedDate <= request.EndFilterDate),
                orderBy: o => o.OrderByDescending(item => item.CreatedDate),
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            return ResponseService<PaginationResponseDTO<Transaction>>.OK(pagination);
        }
    }
}
