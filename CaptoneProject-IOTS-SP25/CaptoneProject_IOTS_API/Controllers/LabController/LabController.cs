using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.DTO.ProductDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_Service.ResponseService;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_API.Controllers.LabController
{
    [Route("api/lab")]
    [ApiController]
    public class LabController : MyBaseController.MyBaseController
    {
        private readonly ILabService labService;

        public LabController(ILabService labService)
        {
            this.labService = labService;
        }

        [HttpPost("member/get-lab-pagination/{comboId}")]
        public async Task<IActionResult> GetLabsPaginationByCombo(int comboId,
            [FromBody] PaginationRequest payload)
        {
            var res = await labService.GetComboLabsPagination(comboId, payload);

            return GetActionResult(res);
        }

        [HttpPost("store-management/get-lab-pagination")]
        [Authorize]
        public async Task<IActionResult> GetLabsPaginationStoreManagement([FromQuery] int? comboId,
            [FromBody] PaginationRequest payload)
        {
            var res = await labService.GetStoreManagementLabsPagination(comboId, payload);

            return GetActionResult(res);
        }

        [HttpPost("trainer-management/get-lab-pagination")]
        [Authorize]
        public async Task<IActionResult> GetLabsPaginationTrainerManagement([FromBody] GenericPaginationRequest<LabFilterRequestDTO> payload)
        {
            var res = await labService.GetTrainerManagementLabsPagination(payload.AdvancedFilter, payload.paginationRequest);

            return GetActionResult(res);
        }

        [HttpPost("admin-manager-management/get-lab-pagination")]
        [Authorize]
        public async Task<IActionResult> GetLabsPaginationAdminOrManagerManagement([FromBody] GenericPaginationRequest<LabFilterRequestDTO> payload)
        {
            var res = await labService.GetLabPagination(payload.AdvancedFilter, payload.paginationRequest);

            return GetActionResult(res);
        }

        [HttpPost("trainer-management/create-lab-information")]
        [Authorize]
        public async Task<IActionResult> CreateLabInformation([FromBody] CreateUpdateLabInformationDTO payload)
        {
            var res = await labService.CreateOrUpdateLabDetailsInformation(null, payload);

            return GetActionResult(res);
        }

        [HttpPut("trainer-management/update-lab-information/{labId}")]
        [Authorize]
        public async Task<IActionResult> UpdateLabInformation(
            int labId,
            [FromBody] CreateUpdateLabInformationDTO payload)
        {
            var res = await labService.CreateOrUpdateLabDetailsInformation(labId, payload);

            return GetActionResult(res);
        }

        [HttpPost("trainer-management/create-or-update-lab-video-playlist/{labId}")]
        [Authorize]
        public async Task<IActionResult> CreateLabVideoPlaylist(
            int labId,
            [FromBody] List<CreateUpdateLabVideo> payload)
        {
            var res = await labService.CreateOrUpdateLabVideoList(labId, payload);

            return GetActionResult(res);
        }

        [HttpGet("get-lab-playlist/{labId}")]
        [Authorize]
        public async Task<IActionResult> GetLabVideoPlaylist(int labId)
        {
            var res = await labService.GetLabVideoList(labId);

            return GetActionResult(res);
        }

        [HttpGet("get-lab-information/{labId}")]
        [Authorize]
        public async Task<IActionResult> GetLabInformation(int labId)
        {
            try
            {
                var res = await labService.GetLabDetailsInformation(labId);

                return GetActionResult(ResponseService<object>.OK(res));
            } catch (Exception ex)
            {
                return GetActionResult(ResponseService<object>.BadRequest(ex.Message));
            }
        }

        [HttpPost("trainer-management/submit-lab/{labId}")]
        public async Task<IActionResult> SubmitLab(int labId)
        {
            var res = await labService.SubmitLabRequest(labId);

            return GetActionResult(res);
        }

        [HttpPost("store-management/approve/{labId}")]
        public async Task<IActionResult> ApproveLab(int labId)
        {
            var res = await labService.ApproveOrRejectLab(labId, true);

            return GetActionResult(res);
        }

        [HttpPost("store-management/reject/{labId}")]
        public async Task<IActionResult> ApproveLab(int labId,
            [FromBody] RemarkDTO payload)
        {
            var res = await labService.ApproveOrRejectLab(labId, true, payload);

            return GetActionResult(res);
        }
    }
}
