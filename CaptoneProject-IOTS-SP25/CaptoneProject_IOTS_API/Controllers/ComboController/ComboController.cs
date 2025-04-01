using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.ComboController
{
    [Route("api/combo")]
    [ApiController]
    public class ComboController : MyBaseController.MyBaseController
    {
        private readonly IComboService comboService;
        private readonly IActivityLogService activityLogService;
        public ComboController(IComboService comboService, IActivityLogService activityLogService)
        {
            this.comboService = comboService;
            this.activityLogService = activityLogService;
        }

        [HttpPost("get-pagination")]
        public async Task<IActionResult> GetComboPagination([FromBody] PaginationRequest payload)
        {
            try
            {
                var res = await comboService.GetPaginationCombos(payload);

                return GetActionResult(ResponseService<object>.OK(res));
            }
            catch (Exception ex)
            {
                return GetActionResult(ResponseService<object>.BadRequest(ex.Message));
            }

        }

        [HttpPost("create-combo")]
        [Authorize]
        public async Task<IActionResult> CreateCombo([FromBody] CreateUpdateComboDTO payload)
        {
            try
            {
                var res = await comboService.CreateOrUpdateCombo(null, payload);

                if (res.IsSuccess)
                {
                    _ = activityLogService.CreateActivityLog($"Create new combo with ID {res?.Data?.Id}");
                }

                return GetActionResult(res);
            }
            catch (Exception ex)
            {
                return GetActionResult(ResponseService<object>.BadRequest(ex.Message));
            }
        }

        [HttpPost("update-combo/{comboId}")]
        [Authorize]
        public async Task<IActionResult> UpdateCombo(int comboId,
            [FromBody] CreateUpdateComboDTO payload)
        {
            try
            {
                var res = await comboService.CreateOrUpdateCombo(comboId, payload);

                if (res.IsSuccess)
                {
                    _ = activityLogService.CreateActivityLog($"Updated combo with ID {comboId}");
                }

                return GetActionResult(res);
            }
            catch (Exception ex)
            {
                return GetActionResult(ResponseService<object>.BadRequest(ex.Message));
            }
        }

        [HttpGet("get-combo-details/{comboId}")]
        public async Task<IActionResult> GetComboDetails(int comboId)
        {
            try
            {
                var res = await comboService.GetComboDetailsById(comboId);

                return GetActionResult(ResponseService<object>.OK(res));
            }
            catch (Exception ex)
            {
                return GetActionResult(ResponseService<object>.BadRequest(ex.Message));
            }
        }

        [HttpPut("combo-status/activate")]
        [Authorize]
        public async Task<IActionResult> ActivateCombo(int comboId)
        {
            var res = await comboService.ActivateOrDeactiveCombo(comboId, true);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Activate combo with ID {comboId}");
            }

            return GetActionResult(res);
        }

        [HttpPut("combo-status/deactivate")]
        [Authorize]
        public async Task<IActionResult> DeactivateCombo(int comboId)
        {
            var res = await comboService.ActivateOrDeactiveCombo(comboId, false);

            if (res.IsSuccess)
            {
                _ = activityLogService.CreateActivityLog($"Deactivated combo with ID {comboId}");
            }

            return GetActionResult(res);
        }
    }
}
