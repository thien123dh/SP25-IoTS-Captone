using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.ProductConst;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_BOs.DTO.FeedbackDTO
{
    public class FeedbackItemRequestDTO
    {
        public int OrderItemId { set; get; }

        public string Comment { set; get; }

        [Range(0.5, 5)]
        public decimal Rating { set; get; }
    }

    public class StoreOrderFeedbackRequestDTO
    {
        public int OrderId { set; get; }
        public int SellerId { set; get; }
        public RoleEnum SellerRole { set; get; }
        public string? BankName { set; get; }
        public string? AccountName { set; get; }
        public string? AccountNumber { set; get; }
        public List<FeedbackItemRequestDTO> FeedbackList { set; get; }
    }

    public class FeedbackResponseDTO
    {
        public int OrderId { set; get; }
        public int OrderItemId { set; get; }
        public int ProductId { set; get; }
        public int ProductType { set; get; }
        public decimal Rating { set; get; }
        public string Comment { set; get; }
        public decimal CreatedByEmail { set; get; }
    }
}
