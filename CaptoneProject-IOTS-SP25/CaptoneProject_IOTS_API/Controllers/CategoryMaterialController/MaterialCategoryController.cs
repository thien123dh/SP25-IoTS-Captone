using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace CaptoneProject_IOTS_API.Controllers.CategoryMaterialController
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialCategoryController : ControllerBase
    {
        private readonly IMaterialCategoryService _materialCategoryService;
        public MaterialCategoryController()
        {
            _materialCategoryService ??= new MatertialCategoryService();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategoryMaterial()
        {
            var result = await _materialCategoryService.GetAllMaterialCategory();
            return Ok(result);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCategoryMaterialById(int Id)
        {
            var result = await _materialCategoryService.GetByMaterialCategoryId(Id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryMaterial ([FromBody] MatertialCategoryRequestDTO matertialCategoryRequest)
        {
            var result = await _materialCategoryService.CreateMaterialCategory(matertialCategoryRequest);
            return Ok(result);
        }

    }
}
