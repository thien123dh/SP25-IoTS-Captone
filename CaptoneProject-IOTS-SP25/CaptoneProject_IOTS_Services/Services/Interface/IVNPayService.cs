using CaptoneProject_IOTS_BOs;
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
        public Task<string> CallAPIPayByUserId(int userId, string returnUrl, long amount);
        public Task<ResponsePayment> GetInformationPayment(int userId, VNPayRequestDTO dto);
    }
}
