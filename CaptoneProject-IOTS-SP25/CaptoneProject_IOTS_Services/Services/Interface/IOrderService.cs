using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IOrderService
    {
        public Task<GenericResponseDTO<OrderResponeDTO>> getOrderDetailsByOrderId(int id);
        public Task<GenericResponseDTO<OrderResponeDTO>> CreateOrder(int? id, OrderRequestDTO payload, string returnUrl);
        public Task<GenericResponseDTO<OrderResponeDTO>> CheckOrderSuccessfull(int? id, VNPayRequestDTO dto);
        public Task<GenericResponseDTO<OrderResponeDTO>> GetOrdersByUserID(int userId);
    }
}
