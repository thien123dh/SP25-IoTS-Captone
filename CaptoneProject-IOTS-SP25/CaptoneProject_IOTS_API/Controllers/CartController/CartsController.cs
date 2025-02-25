using CaptoneProject_IOTS_BOs.DTO.CartDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_Service;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.CartController
{
    [Route("api/cart")]
    [ApiController]
    public class CartsController : MyBaseController.MyBaseController
    {
        private readonly ICartService cartService;

        public CartsController(ICartService cartService) {
            this.cartService = cartService;
        }

        [HttpPost("get-pagination")]
        public async Task<IActionResult> GetCartPagination([FromBody] PaginationRequest payload)
        {
            var res = await cartService.GetCartPagination(payload);

            return GetActionResult(res);
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO payload)
        {
            var res = await cartService.AddToCart(payload);

            return GetActionResult(res);
        }

        [HttpPost("select-or-unselect-cart-item/{cartId}")]
        public async Task<IActionResult> SelectOrUnselectCartItems(int cartId)
        {
            var res = await cartService.SelectOrUnselectCartItem(cartId);

            return GetActionResult(res);
        }

        [HttpDelete("remove-cart-item/{cartId}")]
        public async Task<IActionResult> RemoveCartItem(int cartId)
        {
            var res = await cartService.SelectOrUnselectCartItem(cartId);

            return GetActionResult(res);
        }

        [HttpGet("get-all-combo-included-labs/{cartId}")]
        public async Task<IActionResult> GetAllComboIncludedLabs(int cartId)
        {
            var res = await cartService.GetCartLabItemsByParentId(cartId);

            return GetActionResult(res);
        }

        [HttpGet("get-number-selected-cart-items")]
        public IActionResult GetNumberSelectedCartItems()
        {
            var res = cartService.GetNumberSelectedCartItems();

            return GetActionResult(res);
        }
    }
}
