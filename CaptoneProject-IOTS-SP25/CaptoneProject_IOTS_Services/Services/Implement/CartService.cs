using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.CartDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Mapper;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        private readonly int MAX_RECORD = 500000;

        public CartService(UnitOfWork unitOfWork, IUserServices userService)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
        }

        private GeneralProductDTO GetProductInfo(int productId, int productType)
        {
            GeneralProductDTO res;
            switch (productType)
            {
                case (int)ProductTypeEnum.IOT_DEVICE:
                    var p = unitOfWork.IotsDeviceRepository.GetById(productId);

                    if (p == null)
                        throw new Exception(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);

                    res = new GeneralProductDTO
                    {
                        Name = p.Name,
                        Summary = p.Summary,
                        CreatedBy = p.CreatedBy,
                        Price = p.Price
                    };

                    break;
                case (int)ProductTypeEnum.COMBO:
                    var combo = unitOfWork.ComboRepository.GetById(productId);

                    if (combo == null)
                        throw new Exception(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);

                    res = new GeneralProductDTO
                    {
                        Name = combo.Name,
                        Summary = combo.Summary,
                        CreatedBy = combo.CreatedBy,
                        Price = combo.Price
                    };
                    break;
                case (int)ProductTypeEnum.LAB:
                    var lab = unitOfWork.LabRepository.GetById(productId);

                    if (lab == null)
                        throw new Exception(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);

                    res = new GeneralProductDTO
                    {
                        Name = lab.Title,
                        Summary = lab.Summary,
                        CreatedBy = lab.CreatedBy,
                        Price = lab.Price
                    };

                    break;
                default:
                    throw new Exception(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);
            }

            return res;
        }

        private CartItem SetDataToCartItem(CartItem source, AddToCartDTO request, int loginUserId, int? cartParentId)
        {
            int? sellerId = 0;

            try
            {
                sellerId = GetProductInfo(request.ProductId, (int)request.ProductType).CreatedBy;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            

            source.SellerId = (int)sellerId;
            source.UpdatedDate = DateTime.Now;
            source.CreatedBy = loginUserId;
            source.ParentCartItemId = cartParentId;
            source.Quantity = source.Quantity + request.Quantity;
            source.ProductType = (int)request.ProductType;

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

        public bool CheckExistProductInCart(int userId, int productId, int productType)
        {
            var cartItem = unitOfWork.CartRepository.GetCartItemByProductId(userId, productId, productType);

            return !(cartItem == null);
        }

        private bool CheckProductQuantityAvailable(int cartId, int addToCartQuantity)
        {
            var cart = unitOfWork.CartRepository.GetById(cartId);

            if (cart == null)
                throw new Exception(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);

            switch (cart.ProductType)
            {
                case (int)ProductTypeEnum.IOT_DEVICE:
                    return cart?.IosDeviceNavigation?.Quantity >= addToCartQuantity;

                case (int)ProductTypeEnum.COMBO:
                    return cart?.ComboNavigation?.Quantity >= addToCartQuantity;

                default:
                    throw new Exception("Product type cannot be found. Please try again");
            }
        }

        public async Task<object> AddToCart(AddToCartDTO request)
        {
            var loginUserId = userService.GetLoginUserId();

            if (loginUserId == null)
                throw new Exception(ExceptionMessage.INVALID_LOGIN);

            var cartItem = unitOfWork.CartRepository.GetCartItemByProductId((int)loginUserId, request.ProductId, (int)request.ProductType);

            cartItem = cartItem == null ? new CartItem() : cartItem;

            try
            {
                int? parentCartId = null;

                if (request.ProductType == ProductTypeEnum.LAB)
                {
                    var lab = unitOfWork.LabRepository.GetById(request.ProductId);

                    var parentCartItem = unitOfWork.CartRepository.GetCartItemByProductId((int)loginUserId, lab.ComboId, (int)ProductTypeEnum.COMBO);

                    //Exist combo included lab in cart
                    if (parentCartItem != null)
                    {
                        if (CheckExistProductInCart((int)loginUserId, lab.Id, (int)ProductTypeEnum.LAB))
                        {
                            throw new Exception("The tutorial video has been added already");
                        }

                        parentCartId = parentCartItem.Id;
                        cartItem.IsSelected = parentCartItem.IsSelected;
                    } else
                    {
                        throw new Exception("Please add the current product to cart if you want to buy this tutorial video");
                    }
                }

                cartItem = SetDataToCartItem(cartItem, request, (int)loginUserId, parentCartId);

                if (cartItem.Id > 0 && !CheckProductQuantityAvailable(cartItem.Id, cartItem.Quantity))
                    throw new Exception(ExceptionMessage.Insufficient_product_quantity);

                if (cartItem.Id > 0) //Update
                    cartItem = unitOfWork.CartRepository.Update(cartItem);
                else
                    cartItem = unitOfWork.CartRepository.Create(cartItem);

                if (cartItem.ProductType == (int)ProductTypeEnum.LAB)
                    return cartItem;

                return await GetCartItemById(cartItem.Id);

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private GeneralProductDTO MapToGeneralProductInfo(CartItem item)
        {
            return new GeneralProductDTO
            {
                Name = item.IosDeviceNavigation?.Name ?? item.ComboNavigation?.Name ?? item.LabNavigation?.Title,
                Summary = item.IosDeviceNavigation?.Summary ?? item.ComboNavigation?.Summary ?? item.LabNavigation?.Summary,
                CreatedBy = item.IosDeviceNavigation?.CreatedBy ?? item.ComboNavigation?.CreatedBy ?? item.LabNavigation?.CreatedBy,
                Price = item.IosDeviceNavigation?.Price ?? item.ComboNavigation?.Price ?? item.LabNavigation?.Price ?? 0,
                CreatedByStore = item?.IosDeviceNavigation?.StoreNavigation.Name ?? item?.ComboNavigation?.StoreNavigation?.Name ?? $"Trainer {item?.LabNavigation?.CreatedByNavigation?.Fullname}",
                ImageUrl = item?.IosDeviceNavigation?.ImageUrl ?? item?.ComboNavigation?.ImageUrl ?? item?.LabNavigation?.ImageUrl,
            };
        }

        public async Task<ResponseDTO> GetCartPagination(PaginationRequest request)
        {
            var loginUserId = userService.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_LOGIN);

            var pagination = unitOfWork.CartRepository.GetPaginate(
                filter: item => item.CreatedBy == loginUserId && (
                    item.ProductType == (int)ProductTypeEnum.COMBO || item.ProductType == (int)ProductTypeEnum.IOT_DEVICE
                ),
                orderBy: ob => ob.OrderByDescending(item => item.Id),
                includeProperties: "IosDeviceNavigation,ComboNavigation,LabNavigation,IosDeviceNavigation.StoreNavigation,ComboNavigation.StoreNavigation",
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            var subCartItems = unitOfWork.CartRepository.GetSubItemsListByUserId((int)loginUserId);

            var res = PaginationMapper<CartItem, CartItemResponseDTO>.MapTo((item) =>
            {
                int? productId = (item.IosDeviceId == null) ? item.ComboId : item.IosDeviceId;

                var productInfo = MapToGeneralProductInfo(item);

                var dependLabCartItems = subCartItems?.Where(i => i.ParentCartItemId == item.Id).ToList();

                var sumLabPrice = dependLabCartItems?.Sum(i => i.LabNavigation?.Price == null ? 0 : i.LabNavigation.Price);

                var response = new CartItemResponseDTO
                {
                    IsSelected = item.IsSelected,
                    Id = item.Id,
                    ProductId = item.IosDeviceId != null ? item.IosDeviceId : item.ComboId != null ? item.ComboId : item.LabId,
                    ProductType = item.ProductType,
                    Quantity = item.Quantity,
                    CreatedBy = item.SellerId,
                    Price = productInfo.Price,
                    ProductSummary = productInfo.Summary,
                    ProductName = productInfo.Name,
                    CreatedByStore = productInfo.CreatedByStore,
                    NumberOfIncludedLabs = dependLabCartItems?.Count() == null ? 0 : dependLabCartItems.Count(),
                    ImageUrl = productInfo.ImageUrl,
                    TotalPrice = (productInfo.Price * item.Quantity + (decimal)(sumLabPrice == null ? 0 : sumLabPrice))
                };

                return response;
            }, pagination);

            return ResponseService<object>.OK(res);
        }

        public async Task<ResponseDTO> SelectOrUnselectCartItem(int cartId, bool isSelect)
        {
            var cartItem = unitOfWork.CartRepository.GetById(cartId);

            if (cartItem == null)
                return ResponseService<object>.NotFound(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);

            cartItem.IsSelected = isSelect;

            try
            {
                var dependCartItems = unitOfWork.CartRepository.GetCartItemsListByParentId(cartItem.Id);

                //Auto set selected or unselect dependence lab
                dependCartItems = dependCartItems?.Select(
                    item =>
                    {
                        item.IsSelected = isSelect;
                        return item;
                    }
                )?.ToList();

                if (dependCartItems != null)
                    await unitOfWork.CartRepository.UpdateAsync(dependCartItems);

                unitOfWork.CartRepository.Update(cartItem);
            }
            catch
            {
                return ResponseService<object>.NotFound("Cannot select or unselect product. Please try again");
            }

            return ResponseService<object>.OK(null);
        }

        public async Task<ResponseDTO> DeleteCartItem(int cartId)
        {
            var cartItem = unitOfWork.CartRepository.GetById(cartId);

            if (cartItem == null)
                return ResponseService<object>.NotFound(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);

            try
            {
                var dependCartItems = unitOfWork.CartRepository.GetCartItemsListByParentId(cartItem.Id);

                if (dependCartItems != null)
                    await unitOfWork.CartRepository.RemoveAsync(dependCartItems);

                unitOfWork.CartRepository.Remove(cartItem);
            } catch
            {
                return ResponseService<object>.NotFound("Cannot remove the product. Please try again");
            }

            return ResponseService<object>.OK(null);
        }

        public async Task<List<CartLabItemDTO>?> GetCartLabItemsByParentId(int parentId)
        {
            var cart = unitOfWork.CartRepository.GetById(parentId);

            if (cart == null)
                throw new Exception(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);

            var list = unitOfWork.CartRepository.GetCartItemsListByParentId(parentId);

            var res = list?.Select(item =>
                new CartLabItemDTO
                {
                    CreatedBy = item.CreatedBy,
                    CreatedByTrainer = item?.LabNavigation?.CreatedByNavigation?.Fullname,
                    LabId = item?.LabId,
                    LabName = item?.LabNavigation?.Title,
                    LabSummary = item?.LabNavigation?.Summary,
                    Id = item.Id,
                    Price = item?.LabNavigation?.Price,
                    ImageUrl = item?.LabNavigation?.ImageUrl
                }
            )?.ToList();

            return res;
        }

        public ResponseDTO GetTotalSelectedCartItems()
        {
            var loginUserId = userService.GetLoginUserId();

            //if (loginUserId == null)
            //{
            //    return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_LOGIN);
            //}

            var res = unitOfWork.CartRepository.Search(
                item => item.IsSelected
                &&
                item.CreatedBy == loginUserId
                &&
                item.ParentCartItemId == null
            )?.Count();

            return ResponseService<object>.OK(res == null ? 0 : res);
        }

        public async Task<CartItemResponseDTO> GetCartItemById(int cartId)
        {
            var item = unitOfWork.CartRepository.GetById(cartId);

            if (item == null)
               throw new Exception(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);

            var includedLabs = unitOfWork.CartRepository.GetCartItemsListByParentId(item.Id);

            var productInfo = MapToGeneralProductInfo(item);

            var sumLabPrice = includedLabs?.Sum(i => i.LabNavigation?.Price == null ? 0 : i.LabNavigation.Price);

            var response = new CartItemResponseDTO
            {
                IsSelected = item.IsSelected,
                Id = item.Id,
                ProductId = item.IosDeviceId != null ? item.IosDeviceId : item.ComboId != null ? item.ComboId : item.LabId,
                ProductType = item.ProductType,
                Quantity = item.Quantity,
                CreatedBy = item.SellerId,
                Price = productInfo.Price,
                ProductSummary = productInfo.Summary,
                ProductName = productInfo.Name,
                CreatedByStore = productInfo.CreatedByStore,
                NumberOfIncludedLabs = includedLabs?.Count() == null ? 0 : includedLabs.Count(),
                TotalPrice = (productInfo.Price * item.Quantity + (decimal)(sumLabPrice == null ? 0 : sumLabPrice)),
                ImageUrl = productInfo.ImageUrl
            };

            return response;
        }

        public async Task<ResponseDTO> UpdateCartItemQuantity(UpdateCartQuantityDTO request)
        {
            var loginUserId = userService.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<object>.NotFound(ExceptionMessage.INVALID_LOGIN);

            var cart = unitOfWork.CartRepository.GetById(request.CartId);

            if (cart == null)
                return ResponseService<object>.NotFound(ExceptionMessage.PRODUCT_CANNOT_BE_FOUND);

            cart.Quantity = request.Quantity;

            if (!CheckExistProductInCart((int)loginUserId, (cart.ComboId == null ? (int)cart.IosDeviceId : (int)cart.ComboId), cart.ProductType))
            {
                return ResponseService<object>.BadRequest(ExceptionMessage.Insufficient_product_quantity);
            }

            try
            {
                if (cart.Quantity <= 0)
                    cart.Quantity = 1;

                cart = unitOfWork.CartRepository.Update(cart);

                return ResponseService<object>.OK(await GetCartItemById(cart.Id));

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO> GetCartTotalInformation()
        {
            var loginUserId = userService.GetLoginUserId();

            if (loginUserId == null)
            {
                ResponseService<object>.OK(new
                {
                    TotalCartItemsAmount = 0,
                    TotalSelectedItemsPrice = 0
                });
            }

            int totalCartItemsAmount = unitOfWork.CartRepository
                .Search(item => item.CreatedBy == loginUserId && (
                    item.ProductType == (int)ProductTypeEnum.COMBO || item.ProductType == (int)ProductTypeEnum.IOT_DEVICE
                    )
                ).Count();

            decimal totalSelectedCartItemPrice = unitOfWork.CartRepository
                .Search(item => item.CreatedBy == loginUserId && item.IsSelected).Include(i => i.IosDeviceNavigation).Include(i => i.ComboNavigation).Include(i => i.LabNavigation)
                .Sum(i => (i.IosDeviceId != null ? i.IosDeviceNavigation.Price : (i.ComboId != null ? i.ComboNavigation.Price : i.LabNavigation.Price)) * (i.LabId != null ? 1 : i.Quantity));
            
            return ResponseService<object>.OK(new
            {
                TotalCartItemsAmount = totalCartItemsAmount,
                TotalSelectedItemsPrice = totalSelectedCartItemPrice
            });
        }

        public async Task<ResponseDTO> GetPreviewCartOrders()
        {
            var loginUserId = userService.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<object>.Unauthorize(ExceptionMessage.INVALID_LOGIN);

            var pagination = unitOfWork.CartRepository.GetPaginate(
                filter: item => item.CreatedBy == loginUserId && item.IsSelected,
                orderBy: ob => ob.OrderByDescending(item => item.Id),
                includeProperties: "IosDeviceNavigation,ComboNavigation,LabNavigation,IosDeviceNavigation.StoreNavigation,ComboNavigation.StoreNavigation,LabNavigation.CreatedByNavigation",
                pageIndex: 0,
                pageSize: MAX_RECORD
            );

            var subCartItems = unitOfWork.CartRepository.GetSubItemsListByUserId((int)loginUserId);

            var res = PaginationMapper<CartItem, CartItemResponseDTO>.MapTo((item) =>
            {
                int? productId = item.IosDeviceId ??  item.ComboId ?? item.LabId;

                var productInfo = MapToGeneralProductInfo(item);

                var response = new CartItemResponseDTO
                {
                    IsSelected = item.IsSelected,
                    Id = item.Id,
                    ParentId = item.ParentCartItemId,
                    ProductId = item.IosDeviceId != null ? item.IosDeviceId : item.ComboId != null ? item.ComboId : item.LabId,
                    ProductType = item.ProductType,
                    Quantity = item.Quantity,
                    CreatedBy = item.SellerId,
                    Price = productInfo.Price,
                    ProductSummary = productInfo.Summary,
                    ProductName = productInfo.Name,
                    CreatedByStore = productInfo.CreatedByStore,
                    ImageUrl = productInfo.ImageUrl,
                    TotalPrice = (productInfo.Price * item.Quantity)
                };

                return response;
            }, pagination);

            return ResponseService<object>.OK(res);
        }
    }
}
