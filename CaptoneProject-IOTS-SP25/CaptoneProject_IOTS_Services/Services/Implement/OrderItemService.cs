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

        public async Task<GenericResponseDTO<List<OrderItemResponeDTO>>> getOrderDetailsByOrderId(int id)
        {
            try
            {
                // Lấy danh sách OrderItem theo OrderId
                var orderItems = await _unitOfWork.OrderDetailRepository
                    .GetQueryable(id)
                    .Include(oi => oi.Seller) // Include bảng User để lấy thông tin seller
                    .Where(oi => oi.OrderId == id)
                    .ToListAsync();

                if (orderItems == null || !orderItems.Any())
                    return ResponseService<List<OrderItemResponeDTO>>.NotFound("No order items found.");

                // Lấy thông tin đơn hàng từ bảng Orders
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
                if (order == null)
                    return ResponseService<List<OrderItemResponeDTO>>.NotFound("Order not found.");

                // Chuyển danh sách OrderItems thành DTO
                var orderDetailsDTO = orderItems.Select(od => new OrderItemResponeDTO
                {
                    NameShop = od.Seller != null ? od.Seller.Fullname : "Unknown", // Kiểm tra null để tránh lỗi
                    Quantity = od.Quantity,
                    ProductType = od.ProductType,
                    Price = od.Price,
                    WarrantyEndDate = od.WarrantyEndDate,
                    OrderItemStatus = od.OrderItemStatus
                }).ToList();

                return ResponseService<List<OrderItemResponeDTO>>.OK(orderDetailsDTO);
            }
            catch (Exception ex)
            {
                return ResponseService<List<OrderItemResponeDTO>>.BadRequest("Cannot get order details. Please try again.");
            }
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
