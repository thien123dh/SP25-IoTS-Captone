using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderItemsDTO;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IOrderService
    {
        public Task<GenericResponseDTO<OrderReturnPaymentDTO>> CreateOrder(int? id, OrderRequestDTO payload, string returnUrl);
        public Task<GenericResponseDTO<OrderReturnPaymentDTO>> CheckOrderSuccessfull(int? id, VNPayRequestDTO dto);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseDTO>>> GetOrdersByUserPagination(int? filterOrderId, PaginationRequest payload);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseToStoreDTO>>> getOrderByStoreId(int? filterOrderId, PaginationRequest payload);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseDTO>>> GetAllOrdersPagination(int? filterOrderId, PaginationRequest payload);


    }
}
