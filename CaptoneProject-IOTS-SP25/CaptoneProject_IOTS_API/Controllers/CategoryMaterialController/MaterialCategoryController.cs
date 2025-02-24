using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.InteropServices;

namespace CaptoneProject_IOTS_API.Controllers.CategoryMaterialController
{
    [Route("api/material-category")]
    [ApiController]
    public class MaterialCategoryController : ControllerBase
    {
        private readonly IMaterialCategoryService _materialCategoryService;
        public MaterialCategoryController(
            IFileService fileService,
            IMaterialCategoryService _materialCategoryService
        )
        {
            this._materialCategoryService = _materialCategoryService;
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

        [HttpGet("get-all-material-categories")]
        public async  Task<IActionResult> GetAll([FromQuery] string? searchKeyword)
        {
            var result = await _materialCategoryService.GetAllMaterialCategory(searchKeyword == null ? "" : searchKeyword);

            return GetActionResult(result);
        }

        [HttpPost("get-pagination")]
        public async Task<IActionResult> GetPaginationCategoryMaterial([FromBody] PaginationRequest payload, 
            [FromQuery] int? statusFilter)
        {
            var result = await _materialCategoryService.GetPaginationMaterialCategories(payload, statusFilter);
            return GetActionResult(result);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCategoryMaterialById(int Id)
        {
            var result = await _materialCategoryService.GetByMaterialCategoryId(Id);
            return Ok(result);
        }

        [HttpPost("create-material-category")]
        public async Task<IActionResult> AddCategoryMaterial(
            [FromBody] CreateUpdateMaterialCategoryDTO payload
        )
        {
            var response = await _materialCategoryService.CreateOrUpdateMaterialCategory(null, payload);

            return GetActionResult(response);
        }

        [HttpPost("create-material-category-to-store")]
        public async Task<IActionResult> AddCategoryMaterialToStore(
            [FromBody] CreateUpdateMaterialCategoryDTO payload
        )
        {
            var response = await _materialCategoryService.CreateOrUpdateMaterialCategoryToStore(null, payload);

            return GetActionResult(response);
        }


        [HttpPut("Update-material-category/{id}")]
        public async Task<IActionResult> UpdateMaterialCategory(
            int id,
            [FromBody] CreateUpdateMaterialCategoryDTO payload
        )
        {
            var response = await _materialCategoryService.CreateOrUpdateMaterialCategory(id, payload);

            return GetActionResult(response);
        }

        [HttpPut("Approve-material-category/{id}")]
        public async Task<IActionResult> ActivateMaterialCategory(int id)
        {
            var response = await _materialCategoryService.UpdateMaterialCategoryStatus(id, 1);

            return GetActionResult(response);
        }

        [HttpDelete("Reject-material-category/{id}")]
        public async Task<IActionResult> DeleteMaterialCategory(int id)
        {
            var response = await _materialCategoryService.DeleteMaterialCategory(id);

            return GetActionResult(response);
        }
        
    }
}
