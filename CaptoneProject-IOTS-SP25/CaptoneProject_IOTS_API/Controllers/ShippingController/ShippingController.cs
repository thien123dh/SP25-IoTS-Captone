using CaptoneProject_IOTS_BOs.DTO.GHTKDTO;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace CaptoneProject_IOTS_API.Controllers.ShippingController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IGHTKService _shippingService;
        private readonly HttpClient _httpClient;

        public ShippingController(IGHTKService shippingService, HttpClient httpClient)
        {
            _shippingService = shippingService;
            _httpClient = httpClient;
        }

        [HttpPost("get-fee")]
        public async Task<IActionResult> GetShippingFeeAsync([FromBody] ShippingFeeRequest requestModel)
        {
            if (requestModel == null)
                return BadRequest("Invalid request model");

            var shippingFees = await _shippingService.GetShippingFeeAsync(requestModel);

            if (shippingFees == null || !shippingFees.Any())
                return NotFound("No shipping cost data available");

            return Ok(shippingFees);
        }
    }
}
