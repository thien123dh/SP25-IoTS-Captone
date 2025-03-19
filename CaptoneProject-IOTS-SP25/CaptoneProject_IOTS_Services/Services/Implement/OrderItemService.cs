using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderItemsDTO;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class OrderItemService : IOrderItemService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IUserServices userService;

        public OrderItemService(IUserServices userServices)
        {
            _unitOfWork ??= new UnitOfWork();
            this.userService = userServices;
        }

        public async Task<GenericResponseDTO<List<OrderItemResponeDTO>>> getOrderDetailsByStoreId()
        {
            // Lấy thông tin user đăng nhập
            var loginUser = userService.GetLoginUser();

            // Kiểm tra quyền truy cập
            if (loginUser == null || !await userService.CheckLoginUserRole(RoleEnum.STORE))
                return ResponseService<List<OrderItemResponeDTO>>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            // Lấy danh sách OrderItem của storeId
            var orderItems = await _unitOfWork.OrderDetailRepository
                .GetQueryable()
                .Include(oi => oi.Seller)
                .Where(oi => oi.SellerId == loginUserId) // So sánh với SellerId
                .ToListAsync();

            if (!orderItems.Any())
                return ResponseService<List<OrderItemResponeDTO>>.NotFound("No order items found.");

            // Chuyển danh sách OrderItems thành DTO
            var orderDetailsDTO = orderItems.Select(od => new OrderItemResponeDTO
            {
                NameShop = od.Seller != null ? od.Seller.Fullname : "Unknown",
                
                Quantity = od.Quantity,
                ProductType = od.ProductType,
                Price = od.Price,
                WarrantyEndDate = od.WarrantyEndDate,
                OrderItemStatus = od.OrderItemStatus
            }).ToList();

            return ResponseService<List<OrderItemResponeDTO>>.OK(orderDetailsDTO);
        }

    }
}
