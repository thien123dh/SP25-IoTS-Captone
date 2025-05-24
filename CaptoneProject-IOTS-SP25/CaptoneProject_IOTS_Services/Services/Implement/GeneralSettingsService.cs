using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.GeneralSettingDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class GeneralSettingsService : IGeneralSettingsService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IUserServices _userServices;
        public GeneralSettingsService(UnitOfWork unitOfWork, IUserServices userServices)
        {
            _unitOfWork = unitOfWork;
            _userServices = userServices;
        }

        public async Task<GenericResponseDTO<GeneralSettings>> GetGeneralSettings()
        {
            var res = _unitOfWork.GeneralSettingsRepository.Search(item => true).FirstOrDefault();

            if (res == null)
            {
                ResponseService<GeneralSettings>.BadRequest("General setting not found");
            }

            return ResponseService<GeneralSettings>.OK(res);
        }

        public async Task<GenericResponseDTO<GeneralSettings>> UpdateGeneralSettings(UpdateGeneralSettingRequest settings)
        {
            var generalSettings = _unitOfWork.GeneralSettingsRepository.GetById(settings.Id);
            var loginUserId = (int)_userServices.GetLoginUserId();

            generalSettings.ApplicationFeePercent = settings.ApplicationFeePercent;
            generalSettings.OrderSuccessDays = settings.OrderSuccessDays;
            generalSettings.UpdatedDate = DateTime.Now;
            generalSettings.UpdatedBy = loginUserId;

            if (generalSettings.ApplicationFeePercent <= 0 &&  generalSettings.OrderSuccessDays > 100)
            {
                return ResponseService<GeneralSettings>.BadRequest("Invalid Value. The value must be in [0, 100]");
            }

            _unitOfWork.GeneralSettingsRepository.Update(generalSettings);

            return ResponseService<GeneralSettings>.OK(generalSettings);
        }
    }
}
