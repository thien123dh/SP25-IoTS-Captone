using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.CartDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Mapper;
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

        private GeneralProductDTO GetProductInfo(int productId, int productType)
        {
            GeneralProductDTO res;
            switch (productType)
            {
                case (int)ProductTypeEnum.IOT_DEVICE:
                    var p = unitOfWork.IotsDeviceRepository.GetById(productId);

                    if (p == null)
                        throw new Exception("Product cannot be found");

                    res = new GeneralProductDTO
                    {
                        ProductName = p.Name,
                        ProductSummary = p.Summary,
                        CreatedBy = p.CreatedBy,
                        Price = p.Price
                    };

                    break;
                case (int)ProductTypeEnum.COMBO:
                    var combo = unitOfWork.ComboRepository.GetById(productId);

                    if (combo == null)
                        throw new Exception("Combo cannot be found");

                    res = new GeneralProductDTO
                    {
                        ProductName = combo.Name,
                        ProductSummary = combo.Summary,
                        CreatedBy = combo.CreatedBy,
                        Price = combo.Price
                    };
                    break;
                case (int)ProductTypeEnum.LAB:
                    var lab = unitOfWork.LabRepository.GetById(productId);

                    if (lab == null)
                        throw new Exception("Combo cannot be found");

                    res = new GeneralProductDTO
                    {
                        ProductName = lab.Title,
                        ProductSummary = lab.Summary,
                        CreatedBy = lab.CreatedBy,
                        Price = lab.Price
                    };

                    break;
                default:
                    throw new Exception("Product cannot be found");
            }

            return res;
        }

        private CartItem SetDataToCartItem(CartItem source, AddToCartDTO request, int loginUserId, int? cartParentId)
        {
            int? sellerId = 0;

            try
            {
                sellerId = GetProductInfo(request.ProductId, (int)request.ProductType).CreatedBy;
            } catch
            {
                throw new Exception("Seller cannot be found. Please try again");
            }            

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

        public bool CheckExistProductInCart(int userId, int productId, int productType)
        {
            var cartItem = unitOfWork.CartRepository.GetCartItemByProductId(userId, productId, productType);

            return !(cartItem == null);
        }

        public async Task<ResponseDTO> AddToCart(AddToCartDTO request)
        {
            var loginUserId = userService.GetLoginUserId();

            if (loginUserId == null)
                return ResponseService<object>.BadRequest(ExceptionMessage.INVALID_LOGIN);

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
                        if (CheckExistProductInCart((int)loginUserId, lab.Id, (int)ProductTypeEnum.COMBO))
                        {
                            return ResponseService<object>.BadRequest("The tutorial video has been added to cart");
                        }

                        parentCartId = parentCartItem.Id;
                    } else
                    {
                        return ResponseService<object>.BadRequest("Please add the current product to cart if you want to buy this tutorial video");
                    }
                }

                cartItem = SetDataToCartItem(cartItem, request, (int)loginUserId, parentCartId);

                if (cartItem.Id > 0) //Update
                    cartItem = unitOfWork.CartRepository.Update(cartItem);
                else
                    cartItem = unitOfWork.CartRepository.Create(cartItem);

                return ResponseService<object>.OK(cartItem);
            } catch (Exception ex)
            {
                return ResponseService<object>.BadRequest(ex.Message);
            }
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
                includeProperties: "IosDeviceNavigation,ComboNavigation",
                pageIndex: request.PageIndex,
                pageSize: request.PageSize
            );

            var res = PaginationMapper<CartItem, CartItemResponseDTO>.MappingTo((item) =>
            {
                int? productId = (item.IosDeviceId == null) ? item.ComboId : item.IosDeviceId;

                var productInfo = GetProductInfo(
                    (int)productId,
                    item.ProductType
                );

                var cartLabItems = unitOfWork.CartRepository.GetCartItemsListByParentId(item.Id);

                var sumLabPrice = cartLabItems?.Sum(i => i.LabNavigation.Price);

                var response = new CartItemResponseDTO
                {
                    IsSelected = item.IsSelected,
                    ComboId = item.ComboId,
                    IosDeviceId = item.IosDeviceId,
                    ProductType = item.ProductType,
                    Quantity = item.Quantity,
                    CreatedBy = item.SellerId,
                    Price = productInfo.Price,
                    ProductSummary = productInfo.ProductSummary,
                    ProductName = productInfo.ProductName,
                    labList = cartLabItems?.Select(i => new CartLabItemDTO
                    {
                        IsSelected = i.IsSelected,
                        LabName = i.LabNavigation.Title,
                        LabSummary = i.LabNavigation.Summary,
                        CreatedBy = i.LabNavigation.CreatedBy,
                        Price = i.LabNavigation.Price,
                        LabId = i.LabId,
                        CreatedByTrainer = i.LabNavigation.CreatedByNavigation.Fullname,
                    }).ToList(),
                    TotalPrice = (productInfo.Price * item.Quantity + (decimal)(sumLabPrice == null ? 0 : sumLabPrice))
                };

                return response;
            }, pagination);

            return ResponseService<object>.OK(res);
        }
    }
}
