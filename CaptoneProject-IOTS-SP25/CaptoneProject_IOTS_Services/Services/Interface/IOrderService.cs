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

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IOrderService
    {
        public Task<GenericResponseDTO<OrderReturnPaymentVNPayDTO>> CreateOrder(int? id, OrderRequestDTO payload, string returnUrl);
        public Task<GenericResponseDTO<OrderReturnPaymentDTO>> CheckOrderSuccessfull(int? id, VNPayRequestDTO dto);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseDTO>>> GetOrdersByUserPagination(PaginationRequest payload);
        public Task<GenericResponseDTO<OrderResponseDTO>> GetOrdersDetailsByOrderId(int orderId, OrderItemStatusEnum? orderItemStatusFilter);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseDTO>>> GetOrderByStoreOrTrainerPagination(PaginationRequest payload);
        public Task<ResponseDTO> UpdateOrderDetailToPackingByStoreId(int updateOrderId);
        public Task<ResponseDTO> UpdateOrderDetailToDeliveringByStoreId(int updateOrderId);
        public Task<ResponseDTO> UpdateOrderDetailToPendingToFeedbackByStoreId(int updateOrderId);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseDTO>>> GetAdminOrdersPagination(PaginationRequest payload);
        public Task<GenericResponseDTO<PaginationResponseDTO<OrderResponseToStoreDTO>>> GetOrderByStoreIdHasStatusPending(int? filterOrderId, PaginationRequest payload);


    }
}
