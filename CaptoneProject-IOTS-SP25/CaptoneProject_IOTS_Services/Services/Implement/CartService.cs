using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.CartDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class CartService : ICartService
    {
        private readonly UnitOfWork unitOfWork;
        private readonly IUserServices userService;

        public CartService(UnitOfWork unitOfWork, IUserServices userService)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
        }

        private int? GetProductSeller(int productId, int productType)
        {
            int? sellerId;

            switch (productType)
            {
                case (int)ProductTypeEnum.IOT_DEVICE:
                    sellerId = unitOfWork.IotsDeviceRepository.GetById(productId)?.CreatedBy;
                    break;
                case (int)ProductTypeEnum.COMBO:
                    sellerId = unitOfWork.ComboRepository.GetById(productId)?.CreatedBy;
                    break;
                case (int)ProductTypeEnum.LAB:
                    sellerId = unitOfWork.LabRepository.GetById(productId)?.CreatedBy;
                    break;
                default:
                    sellerId = null;
                    break;
            }

            return sellerId;
        }

        private CartItem SetDataToCartItem(CartItem source, AddToCartDTO request, int loginUserId, int? cartParentId)
        {
            var sellerId = GetProductSeller(request.ProductId, (int)request.ProductType);

            if (sellerId == null)
                throw new Exception("Seller cannot be found. Please try again");

            source.SellerId = (int)sellerId;
            source.UpdatedDate = DateTime.Now;
            source.CreatedBy = loginUserId;
            source.ParentCartItemId = cartParentId;
            source.Quantity = source.Quantity + request.Quantity;
           

            switch (request.ProductType)
            {
                case ProductTypeEnum.IOT_DEVICE:
                    source.IosDeviceId = request.ProductId;
                    break;
                case ProductTypeEnum.COMBO:
                    source.ComboId = request.ProductId;
                    break;
                case ProductTypeEnum.LAB:
                    source.LabId = request.ProductId;
                    break;
                default:
                    throw new Exception("Cannot Add to Cart this Product. Please try again");
            }

            return source;
        }

        //public async Task<bool> CreateOrUpdateLabCartItem(List<int> LabIdsList, int cartParentId, int userId)
        //{
        //    var dbCartLabItems = unitOfWork.CartRepository.GetAllCartItemsByUserId(userId, (int)ProductTypeEnum.LAB);

        //    var removeList = dbCartLabItems?.Where(item => !LabIdsList.Contains(item.Id)).ToList();

        //    if (removeList != null)
        //        await unitOfWork.CartRepository.RemoveAsync(removeList);

        //    var appendList = LabIdsList.Where(
        //        i => dbCartLabItems == null || dbCartLabItems.FirstOrDefault(item => item.LabId == i) == null
        //    ).Select(i =>
        //        SetDataToCartItem(new CartItem(), new AddToCartDTO
        //        {
        //            ProductId = i,
        //            Quantity = 1,
        //            ProductType = ProductTypeEnum.LAB
        //        }, userId, cartParentId)
        //    ).ToList();

        //    try
        //    {
        //        await unitOfWork.CartRepository.CreateAsync(appendList);

        //        return true;
        //    } catch
        //    {
        //        return false;
        //    }
        //}

        public async Task<ResponseDTO> AddToCart(AddToCartDTO request)
        {
            var loginUserId = userService.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<object>.BadRequest(ExceptionMessage.INVALID_LOGIN);

            var cartItem = unitOfWork.CartRepository.GetCartItemByProductId((int)loginUserId, request.ProductId, (int)request.ProductType);

            cartItem = cartItem == null ? new CartItem() : cartItem;

            try
            {
                cartItem = SetDataToCartItem(cartItem, request, (int)loginUserId, null);

                if (cartItem.Id > 0) //Update
                    cartItem = unitOfWork.CartRepository.Update(cartItem);
                else
                    cartItem = unitOfWork.CartRepository.Create(cartItem);

                //request.LabIdsList = request.LabIdsList == null ? new List<int>() : request.LabIdsList;

                ////If add more lab
                //await CreateOrUpdateLabCartItem(request.LabIdsList, cartItem.Id, (int)loginUserId);

                return ResponseService<object>.OK(cartItem);
            } catch (Exception ex)
            {
                return ResponseService<object>.BadRequest(ex.Message);
            }
        }

        public GenericResponseDTO<PaginationResponseDTO<CartItemResponseDTO>> GetCartPagination(PaginationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
