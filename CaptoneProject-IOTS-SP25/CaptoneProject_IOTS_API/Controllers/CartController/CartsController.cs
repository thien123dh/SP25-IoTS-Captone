using CaptoneProject_IOTS_BOs.DTO.CartDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_Service;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> GetCartPagination([FromBody] PaginationRequest payload)
        {
            var res = await cartService.GetCartPagination(payload);

            return GetActionResult(res);
        }

        [HttpPost("add-to-cart")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO payload)
        {
            try
            {
                var res = await cartService.AddToCart(payload);

                return GetActionResult(ResponseService<object>.OK(res));
            } catch (Exception ex)
            {
                return GetActionResult(ResponseService<object>.BadRequest(ex.Message));
            }
        }

        [HttpPost("select-cart-item/{cartId}")]
        [Authorize]
        public async Task<IActionResult> SelectOrUnselectCartItems(int cartId)
        {
            var res = await cartService.SelectOrUnselectCartItem(cartId, isSelect: true);

            return GetActionResult(res);
        }

        [HttpPost("unselect-cart-item/{cartId}")]
        [Authorize]
        public async Task<IActionResult> UnselectOrUnselectCartItems(int cartId)
        {
            var res = await cartService.SelectOrUnselectCartItem(cartId, isSelect: false);

            return GetActionResult(res);
        }

        [HttpDelete("remove-cart-item/{cartId}")]
        [Authorize]
        public async Task<IActionResult> RemoveCartItem(int cartId)
        {
            var res = await cartService.DeleteCartItem(cartId);

            return GetActionResult(res);
        }

        [HttpGet("get-all-combo-included-labs/{cartId}")]
        [Authorize]
        public async Task<IActionResult> GetAllComboIncludedLabs(int cartId)
        {
            try
            {
                var res = await cartService.GetCartLabItemsByParentId(cartId);

                return GetActionResult(ResponseService<object>.OK(res));
            }
            catch (Exception ex)
            {
                return GetActionResult(ResponseService<object>.BadRequest(ex.Message));
            }
        }

        [HttpGet("get-number-selected-cart-items")]
        [Authorize]
        public IActionResult GetNumberSelectedCartItems()
        {
            var res = cartService.GetTotalSelectedCartItems();

            return GetActionResult(res);
        }

        [HttpGet("get-cart-item-by-id/{cartId}")]
        [Authorize]
        public async Task<IActionResult> GetCartItemById(int cartId)
        {
            try
            {
                var res = await cartService.GetCartItemById(cartId);

                return GetActionResult(ResponseService<object>.OK(res));
            } catch (Exception ex)
            {
                return GetActionResult(ResponseService<object>.BadRequest(ex.Message));
            }
        }

        [HttpPut("update-cart-quantity")]
        [Authorize]
        public async Task<IActionResult> UpdateCartQuantity([FromBody] UpdateCartQuantityDTO payload)
        {
            var res = await cartService.UpdateCartItemQuantity(payload);

            return GetActionResult(res);
        }

        [HttpGet("get-cart-total-information")]
        [Authorize]
        public async Task<IActionResult> GetCartTotalInformation()
        {
            var res = await cartService.GetCartTotalInformation();

            return GetActionResult(res);
        }
    }
}
