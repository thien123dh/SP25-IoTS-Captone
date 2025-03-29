using CaptoneProject_IOTS_BOs.DTO.CommonDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.CommonController
{
    [Route("api/common")]
    [ApiController]
    public class CommonController : MyBaseController.MyBaseController
    {
        private readonly ICommonService commonService;

        public CommonController(ICommonService commonService)
        {
            this.commonService = commonService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> ApplicationSearch(
            [FromBody] CommonSearchRequestDTO payload)
        {
            var res = await commonService.RelativeSearchApplication(payload);

            return GetActionResult(res);
        }
    }
}
