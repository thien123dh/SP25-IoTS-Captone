using CaptoneProject_IOTS_BOs.DTO.VNPayDTO;
using CaptoneProject_IOTS_Service.Business;
using CaptoneProject_IOTS_Service.Services.Interface;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class VNPayService : IVNPayService
    {
        private readonly UnitOfWork _unitOfWork;

        public VNPayService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<IBusinessResult> CallAPIPayByUserId(int userId, string returnUrl, string paymentType)
        {
            try
            {
                var result = await _unitOfWork.payRepository.CallAPIPayByUserId(userId, returnUrl, paymentType);

                if (result != null)
                {
                    return new BusinessResult(1, "success", result);
                }
                else
                {
                    return new BusinessResult(2, "fail");
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(-4, ex.Message);

            }
        }

        public async Task<IBusinessResult> GetInformationPayment(int userId, VNPayRequestDTO dto)
        {
            try
            {
                var result = await _unitOfWork.payRepository.GetInformationPayment(userId, dto);

                if (result != null)
                {
                    return new BusinessResult(1, "success", result);
                }
                else
                {
                    return new BusinessResult(2, "fail");
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(-4, ex.Message);

            }
        }
    }
}
