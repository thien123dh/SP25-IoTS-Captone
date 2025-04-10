using CaptoneProject_IOTS_BOs.DTO.DashboardDTO.Request;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CaptoneProject_IOTS_BOs.Constant.UserEnumConstant;

namespace CaptoneProject_IOTS_API.Controllers.DashboardController
{
    [Route("api/statistic")]
    [Authorize]
    public class StatisticController : MyBaseController.MyBaseController
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpPost]
        [Route("admin/get")]
        //[Authorize(Roles = RoleConst.ADMIN)]
        public async Task<IActionResult> GetAdminStatistic([FromBody] StatisticRequest payload)
        {
            var res = await _statisticService.GetAdminStatistic(payload).ConfigureAwait(false);

            return GetActionResult(res);
        }

        [HttpPost]
        [Route("store/get")]
        [Authorize(Roles = RoleConst.STORE)]
        public async Task<IActionResult> GetStoreStatistic([FromBody] StatisticRequest payload)
        {
            var res = await _statisticService.GetStoreStatistic(payload).ConfigureAwait(false);

            return GetActionResult(res);
        }

        [HttpPost]
        [Route("trainer/get")]
        [Authorize(Roles = RoleConst.TRAINER)]
        public async Task<IActionResult> GetTrainerStatistic([FromBody] StatisticRequest payload)
        {
            var res = await _statisticService.GetTrainerStatistic(payload).ConfigureAwait(false);

            return GetActionResult(res);
        }
    }
}
