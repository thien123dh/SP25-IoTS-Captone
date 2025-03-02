using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaptoneProject_IOTS_BOs.DTO.OrderDTO;
using CaptoneProject_IOTS_BOs.DTO.OrderItemsDTO;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IOrderItemService
    {
        public Task<GenericResponseDTO<List<OrderItemResponeDTO>>> getOrderDetailsByOrderId(int id);

        public Task<GenericResponseDTO<List<OrderItemResponeDTO>>> getOrderDetailsByStoreId();
    }
}
