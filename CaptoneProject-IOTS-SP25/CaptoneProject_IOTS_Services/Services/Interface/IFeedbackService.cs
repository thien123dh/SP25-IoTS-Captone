using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.FeedbackDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IFeedbackService
    {
        Task<ResponseDTO> GetFeedbackPagination(ProductRequestDTO productRequest, PaginationRequest paginationRequest);
        Task<ResponseDTO> CreateOrderFeedback(StoreOrderFeedbackRequestDTO request);
    }
}
