using CaptoneProject_IOTS_BOs.DTO.GHTKDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.LocationController
{
    [Route("api/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IGHTKService _ghtkService;

        public LocationController(IGHTKService ghtkService)
        {
            _ghtkService = ghtkService;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _ghtkService.SyncProvincesAsync();
            return Ok(provinces);
        }

        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            var provinces = await _ghtkService.SyncDistrictsAsync(provinceId);
            return Ok(provinces);
        }

        [HttpGet("wards")]
        public async Task<IActionResult> GetWards(int prodistrictId)
        {
            var provinces = await _ghtkService.SyncWardsAsync(prodistrictId);
            return Ok(provinces);
        }

        [HttpGet("address")]
        public async Task<IActionResult> GetAddress(int prowardId)
        {
            var provinces = await _ghtkService.SyncAddressAsync(prowardId);
            return Ok(provinces);
        }
    }
}
