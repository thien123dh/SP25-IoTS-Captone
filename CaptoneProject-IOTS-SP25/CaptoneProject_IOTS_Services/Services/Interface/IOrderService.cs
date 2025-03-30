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
using CaptoneProject_IOTS_BOs.Constant;
using CaptoneProject_IOTS_BOs.DTO.RefundDTO;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IOrderService
    {
        public Task<GenericResponseDTO<OrderReturnPaymentVNPayDTO>> CreateOrder(int? id, OrderRequestDTO payload, string returnUrl);
        public Task<GenericResponseDTO<OrderReturnPaymentDTO>> CheckOrderSuccessfull(int? id, VNPayRequestDTO dto);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseDTO>>> GetOrdersByUserPagination(PaginationRequest payload, OrderItemStatusEnum? orderItemStatusFilter = null);
        public Task<GenericResponseDTO<OrderResponseDTO>> GetOrdersDetailsByOrderId(int orderId, OrderItemStatusEnum? orderItemStatusFilter);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseDTO>>> GetOrderByStoreOrTrainerPagination(PaginationRequest payload, OrderItemStatusEnum? orderItemStatusFilter = null);
        public Task<ResponseDTO> UpdateOrderDetailToCancel(int orderId, CreateRefundRequestDTO request);
        public Task<ResponseDTO> UpdateOrderDetailToPacking(int updateOrderId);
        public Task<ResponseDTO> UpdateOrderDetailToDelivering(int updateOrderId);

        public Task<ResponseDTO> UpdateOrderDetailToSuccess(int updateOrderId);
        public Task<ResponseDTO> UpdateOrderDetailToPendingToFeedback(int updateOrderId, int sellerId);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseDTO>>> GetAdminOrdersPagination(PaginationRequest payload, OrderItemStatusEnum? orderItemStatusFilter = null);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseToStoreDTO>>> GetOrderByStoreIdHasStatusPending(int? filterOrderId, PaginationRequest payload);
    }
}
