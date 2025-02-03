using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CaptoneProject_IOTS_API.Controllers.MaterialController
{
    [Route("api/material")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        public MaterialController(
            IMaterialService _materialService
        )
        {
            this._materialService = _materialService;
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

        [HttpGet("get-all-active-material")]
        public async Task<IActionResult> GetAll([FromQuery] string? searchKeyword)
        {
            var result = await _materialService.GetAllMaterial(searchKeyword == null ? "" : searchKeyword);

            return GetActionResult(result);
        }

        [HttpPost("get-pagination")]
        public async Task<IActionResult> GetPaginationMaterial([FromBody] PaginationRequest payload)
        {
            var result = await _materialService.GetPaginationMaterial(payload);
            return GetActionResult(result);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetMaterialById(int Id)
        {
            var result = await _materialService.GetByMaterialId(Id);

            return GetActionResult(result);
        }

        [HttpPost("create-material")]
        public async Task<IActionResult> AddMaterial(
            [FromBody] CreateUpdateMaterialDTO payload
        )
        {
            var response = await _materialService.CreateOrUpdateMaterial(null, payload);

            return GetActionResult(response);
        }

        [HttpPut("update-material-/{id}")]
        public async Task<IActionResult> UpdateMaterial(
            int id,
            [FromBody] CreateUpdateMaterialDTO payload
        )
        {
            var response = await _materialService.CreateOrUpdateMaterial(id, payload);

            return GetActionResult(response);
        }

        [HttpPut("activate-material-category/{id}")]
        public async Task<IActionResult> ActivateMaterial(int id)
        {
            var response = await _materialService.UpdateMaterialStatus(id, 1);

            return GetActionResult(response);
        }

        [HttpPut("deactive-material-category/{id}")]
        public async Task<IActionResult> DeactiveMaterial(int id)
        {
            var response = await _materialService.UpdateMaterialStatus(id, 0);

            return GetActionResult(response);
        }
    }
}


