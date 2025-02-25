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
        public Task<ResponseDTO> GetCartPagination(PaginationRequest request);

        public Task<GenericResponseDTO<List<CartLabItemDTO>?>> GetCartLabItemsByParentId(int parentId);

        public Task<ResponseDTO> AddToCart(AddToCartDTO request);

        public Task<ResponseDTO> SelectOrUnselectCartItem(int cartId);

        public Task<ResponseDTO> DeleteCartItem(int cartId);

        public ResponseDTO GetNumberSelectedCartItems();
    }
}
