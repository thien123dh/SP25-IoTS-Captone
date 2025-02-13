using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CaptoneProject_IOTS_BOs.DTO.MaterialGroupCategoryDTO;

namespace CaptoneProject_IOTS_API.Controllers.MaterialGroupCategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialGroupCategoryController : ControllerBase
    {
        private readonly IMaterialGroupCategoryService _materialGroupCategoryService;
        public MaterialGroupCategoryController(
            IFileService fileService,
            IMaterialGroupCategoryService _materialGroupCategoryService
        )
        {
            this._materialGroupCategoryService = _materialGroupCategoryService;
        }

        private IActionResult GetActionResult(ResponseDTO response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(response);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("get-all-active-material-group-categories")]
        public async Task<IActionResult> GetAll([FromQuery] string? searchKeyword)
        {
            var result = await _materialGroupCategoryService.GetAllMaterialGroupCategory(searchKeyword == null ? "" : searchKeyword);

            return GetActionResult(result);
        }

        [HttpPost("get-pagination-material-group-category")]
        public async Task<IActionResult> GetPaginationCategoryMaterialGroup([FromBody] PaginationRequest payload,
            [FromQuery] int? statusFilter)
        {
            var result = await _materialGroupCategoryService.GetPaginationMaterialGroupCategories(payload, statusFilter);
            return GetActionResult(result);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCategoryMaterialGroupById(int Id)
        {
            var result = await _materialGroupCategoryService.GetByMaterialGroupCategoryId(Id);
            return Ok(result);
        }

        [HttpPost("create-material-group-category")]
        public async Task<IActionResult> AddCategoryMaterialGroup(
            [FromBody] CreateUpdateMaterialGroupCategoryDTO payload
        )
        {
            var response = await _materialGroupCategoryService.CreateOrUpdateMaterialGroupCategory(null, payload);

            return GetActionResult(response);
        }

        [HttpPut("update-material-group-category/{id}")]
        public async Task<IActionResult> UpdateMaterialGroupCategory(
            int id,
            [FromBody] CreateUpdateMaterialGroupCategoryDTO payload
        )
        {
            var response = await _materialGroupCategoryService.CreateOrUpdateMaterialGroupCategory(id, payload);

            return GetActionResult(response);
        }

        [HttpPut("activate-material-group-category/{id}")]
        public async Task<IActionResult> ActivateMaterialCategory(int id)
        {
            var response = await _materialGroupCategoryService.UpdateMaterialGroupCategoryStatus(id, 1);

            return GetActionResult(response);
        }

        [HttpPut("deactive-material-group-category/{id}")]
        public async Task<IActionResult> DeactiveMaterialCategory(int id)
        {
            var response = await _materialGroupCategoryService.UpdateMaterialGroupCategoryStatus(id, 0);

            return GetActionResult(response);
        }
    }
}
