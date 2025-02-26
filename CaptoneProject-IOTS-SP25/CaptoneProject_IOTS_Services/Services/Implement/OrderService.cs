using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.WalletDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IUserServices userServices;
        private readonly IVNPayService vnpayServices;

        public OrderService(IUserServices userServices, IVNPayService vnpayServices)
        {
            _unitOfWork ??= new UnitOfWork();
            this.userServices = userServices;
            this.vnpayServices = vnpayServices;
        }

        private string GetApplicationSerialNumberOrder(int userID, string serialNumber)
        {
            string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            return "DV{UserId}{DeviceType}{SerialNumber}{CurrentDateTime}"
                .Replace("{SerialNumber}", serialNumber)
                .Replace("{UserId}", userID.ToString())
                .Replace("{CurrentDateTime}", currentDateTime);
        }


        public async Task<GenericResponseDTO<OrderResponeDTO>> CheckOrderSuccessfull(int? id, OrderRequestDTO payload)
        {
            var loginUser = userServices.GetLoginUser();

            if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER))
                return ResponseService<OrderResponeDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var userAddress = payload.Address;
            var userContactPhone = payload.ContactNumber;
            var userNotes = payload.Notes;

            var selectedItems = await _unitOfWork.CartRepository
                .GetQueryable((int)loginUserId)
                .Where(item => item.CreatedBy == loginUserId && item.IsSelected)
                .Include(item => item.IosDeviceNavigation)
                .ToListAsync();

            var totalPrice = selectedItems.Sum(item =>
                (item.IosDeviceNavigation?.Price ?? 0m) * item.Quantity
            );
            var serialNumber = "OR";
            


            var createTransactionPayment = new Orders
            {
                ApplicationSerialNumber = GetApplicationSerialNumberOrder(loginUserId,serialNumber),
                OrderBy = loginUserId,
                TotalPrice = totalPrice,
                Address = userAddress,
                ContactNumber = userContactPhone,
                Notes = userNotes,
                CreateDate = DateTime.Now,
                CreatedBy = loginUserId,
                UpdatedBy = loginUserId,
                OrderStatusId = (int)OrderStatusEnum.PENDING
            };

            await _unitOfWork.OrderRepository.SaveAsync();


            /*decimal ItemPrice = 0;
            foreach(var item in selectedItems)
            {
                decimal itemPrice = (item.IosDeviceNavigation?.Price ?? 0m) * item.Quantity;
                var orderDetail = new OrderItem
                {
                    OrderId = createTransactionPayment.Id, 
                    IosDeviceId = item.IosDeviceNavigation?.Id ?? 0,
                    ComboId = item.ComboNavigation?.Id ?? 0,
                    LabId = item.LabNavigation?.Id ?? 0,
                    SellerId = item.SellerId,
                    Price = item.IosDeviceNavigation?.Price ?? 0m,
                    TotalPrice = itemPrice,
                    CreatedBy = loginUserId,
                    CreatedDate = DateTime.Now
                };
            }*/



            _unitOfWork.CartRepository.RemoveAsync(selectedItems);
            await _unitOfWork.CartRepository.SaveAsync();

            return new GenericResponseDTO<OrderResponeDTO>
            {
                IsSuccess = true,
                Message = "Đơn hàng đã được xác nhận thành công và sản phẩm đã được xoá khỏi giỏ hàng.",
                Data = null
            };
        }

        public async Task<GenericResponseDTO<OrderResponeDTO>> CreateOrder(int? id, OrderRequestDTO payload)
        {
            var loginUser = userServices.GetLoginUser();

            if (loginUser == null || !await userServices.CheckLoginUserRole(RoleEnum.CUSTOMER))
                return ResponseService<OrderResponeDTO>.Unauthorize("You don't have permission to access");

            var loginUserId = loginUser.Id;

            var address = payload.Address;
            var contactPhone = payload.ContactNumber;
            var notes = payload.Notes;

            // Lấy danh sách sản phẩm được chọn trong giỏ hàng
            var selectedItems = await _unitOfWork.CartRepository
                .GetQueryable((int)loginUserId)
                .Where(item => item.CreatedBy == loginUserId && item.IsSelected)
                .Include(item => item.IosDeviceNavigation)
                .ToListAsync();

            if (selectedItems == null || !selectedItems.Any())
            {
                return new GenericResponseDTO<OrderResponeDTO>
                {
                    IsSuccess = false,
                    Message = "Giỏ hàng trống hoặc không có sản phẩm nào được chọn.",
                    Data = null
                };
            }

            var totalPrice = selectedItems.Sum(item =>
                (item.IosDeviceNavigation?.Price ?? 0m) * item.Quantity
            );

            if (totalPrice <= 0)
            {
                return new GenericResponseDTO<OrderResponeDTO>
                {
                    IsSuccess = false,
                    Message = "Giá trị đơn hàng không hợp lệ.",
                    Data = null
                };
            }

            //Thanh toán VNPay
            string orderProduct = await vnpayServices.CallAPIPayByUserId(
                loginUserId,
                "https://localhost:44346/checkout-process-product",
                (long)totalPrice
            );

            return new GenericResponseDTO<OrderResponeDTO>
            {
                IsSuccess = true,
                Message = orderProduct
            };
        }



        public Task<GenericResponseDTO<OrderResponeDTO>> getOrderDetailsByOrderId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponseDTO<OrderResponeDTO>> GetOrdersByUserID(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
