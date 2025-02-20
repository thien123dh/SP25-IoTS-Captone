using CaptoneProject_IOTS_BOs.DTO.MaterialCategotyDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CaptoneProject_IOTS_BOs.DTO.BlogCategoryDTO;

namespace CaptoneProject_IOTS_API.Controllers.BlogCategoryController
{
    [Route("api/category-blog")]
    [ApiController]
    public class BlogCategoryController : ControllerBase
    {
        private readonly IBlogService _blogCategoryService;
        public BlogCategoryController(IBlogService blogCategoryService)
        {
            this._blogCategoryService = blogCategoryService;
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

        [HttpGet("get-all-active-blog-categories")]
        public async Task<IActionResult> GetAll([FromQuery] string? searchKeyword)
        {
            var result = await _blogCategoryService.GetAllBlogCategory(searchKeyword == null ? "" : searchKeyword);

            return GetActionResult(result);
        }

        [HttpPost("get-pagination")]
        public async Task<IActionResult> GetPaginationCategoryBlog([FromBody] PaginationRequest payload,
            [FromQuery] int? statusFilter)
        {
            var result = await _blogCategoryService.GetPaginationBlogCategories(payload, statusFilter);
            return GetActionResult(result);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCategoryBlogById(int Id)
        {
            var result = await _blogCategoryService.GetByBlogCategoryId(Id);
            return Ok(result);
        }

        [HttpPost("create-blog-category")]
        public async Task<IActionResult> AddCategoryBlog(
            [FromBody] CreateUpdateBlogCategoryDTO payload
        )
        {
            var response = await _blogCategoryService.CreateOrUpdateBlogCategory(null, payload);

            return GetActionResult(response);
        }

        [HttpPut("update-blog-category/{id}")]
        public async Task<IActionResult> UpdateMaterialCategory(
            int id,
            [FromBody] CreateUpdateBlogCategoryDTO payload
        )
        {
            var response = await _blogCategoryService.CreateOrUpdateBlogCategory(id, payload);
            return GetActionResult(response);
        }

        [HttpPut("activate-blog-category/{id}")]
        public async Task<IActionResult> ActivateBlogCategory(int id)
        {
            var response = await _blogCategoryService.UpdateBlogCategoryStatus(id, 1);
            return GetActionResult(response);
        }

        [HttpPut("deactive-blog-category/{id}")]
        public async Task<IActionResult> DeactiveBlogCategory(int id)
        {
            var response = await _blogCategoryService.UpdateBlogCategoryStatus(id, 0);

            return GetActionResult(response);
        }

    }
}
