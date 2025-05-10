using CaptoneProject_IOTS_BOs.DTO.GHTKDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CaptoneProject_IOTS_API.Controllers.MyBaseController;
using Microsoft.AspNetCore.Authorization;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;
using System.Diagnostics.Contracts;
using CaptoneProject_IOTS_BOs.DTO.GeneralSettingDTO;

namespace CaptoneProject_IOTS_API.Controllers.GHTKController
{
    [Route("api/general-settings")]
    public class GeneralSettingController : MyBaseController.MyBaseController
    {
        private readonly IGeneralSettingsService _generalSettingsService;

        public GeneralSettingController(IGeneralSettingsService generalSettingsService)
        {
            _generalSettingsService = generalSettingsService;
        }

        [HttpGet("get")]
        [Authorize(Roles = RoleConst.ADMIN)]
        public async Task<IActionResult> GetGeneralSetting()
        {
            var res = await _generalSettingsService.GetGeneralSettings().ConfigureAwait(false);

            return GetActionResult(res);
        }

        [HttpPut("update")]
        [Authorize(Roles = RoleConst.ADMIN)]
        public async Task<IActionResult> UpdateGeneralSettings([FromBody] UpdateGeneralSettingRequest payload)
        {
            var res = await _generalSettingsService.UpdateGeneralSettings(payload).ConfigureAwait(false);

            return GetActionResult(res);
        }
    }
}
