using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.CartDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface ICartService
    {
        public GenericResponseDTO<PaginationResponseDTO<CartItemResponseDTO>> GetCartPagination(PaginationRequest request);

        public Task<ResponseDTO> AddToCart(AddToCartDTO request);
    }
}
