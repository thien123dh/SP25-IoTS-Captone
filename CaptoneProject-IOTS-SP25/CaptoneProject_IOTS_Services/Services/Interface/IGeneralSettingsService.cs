using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.GeneralSettingDTO;
using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Interface
{
    public interface IGeneralSettingsService
    {
        Task<GenericResponseDTO<GeneralSettings>> UpdateGeneralSettings(UpdateGeneralSettingRequest settings);

        Task<GenericResponseDTO<GeneralSettings>> GetGeneralSettings();
    }
}
