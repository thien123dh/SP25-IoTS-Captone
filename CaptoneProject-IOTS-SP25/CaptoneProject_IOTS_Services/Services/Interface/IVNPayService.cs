using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
using CaptoneProject_IOTS_Service.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IVNPayService
    {
        public Task<IBusinessResult> CallAPIPayByUserId(int userId, string returnUrl, string paymentType);
        public Task<IBusinessResult> GetInformationPayment(int userId, VNPayRequestDTO dto);
    }
}
