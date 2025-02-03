using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.DTO.MaterialDTO;
using CaptoneProject_IOTS_BOs.DTO.PaginationDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CaptoneProject_IOTS_API.Controllers.ProductRequestController
{
    [Route("api/product-request")]
    [ApiController]
    public class ProductRequestController : ControllerBase
    {

        private readonly IProductRequestService productRequestService;

        public ProductRequestController(IProductRequestService productRequestService)
        {
            this.productRequestService = productRequestService;
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

        [HttpPost("get-paginate")]
        public async Task<IActionResult> GetPagination([FromBody] PaginationRequest payload)
        {
            var response = await productRequestService.GetPaginationProductRequest(payload);

            return GetActionResult(response);
        }

        [HttpPost("submit-material-request")]

        public async Task<IActionResult> SubmitMaterialRequest([FromQuery] int? productRequestId,
            [FromBody] CreateUpdateMaterialDTO payload)
        {
            var response = await productRequestService.SubmitMaterialRequest(productRequestId, payload);

            return GetActionResult(response);
        }

        [HttpGet("get-product-request-by-id/{id}")]
        public async Task<IActionResult> GetByProductRequestById(int id)
        {
            var response = await productRequestService.GetProductRequestById(id);

            return GetActionResult(response);
        }

    }
}
