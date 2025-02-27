using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.ComboController
{
    [Route("api/combo")]
    [ApiController]
    public class ComboController : MyBaseController.MyBaseController
    {
        private readonly IComboService comboService;

        public ComboController(IComboService comboService)
        {
            this.comboService = comboService;
        }

        [HttpPost("get-pagination")]
        public async Task<IActionResult> GetComboPagination([FromBody] PaginationRequest payload)
        {
            try
            {
                var res = await comboService.GetPaginationCombos(payload);

                return GetActionResult(ResponseService<object>.OK(res));
            } catch (Exception ex)
            {
                return GetActionResult(ResponseService<object>.BadRequest(ex.Message));
            }
            
        }

        [HttpPost("create-combo")]
        public async Task<IActionResult> CreateCombo([FromBody] CreateUpdateComboDTO payload)
        {
            try
            {
                var res = await comboService.CreateOrUpdateCombo(null, payload);

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
    }
}
