using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.GHTKController
{
    [Route("api/ghtk")]
    [ApiController]
    public class GHTKController : ControllerBase
    {
        private readonly IGHTKService _ghtkService;

        public GHTKController(IGHTKService ghtkService)
        {
            _ghtkService = ghtkService;
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetTrackingOrder(int orderId)
        {
            try
            {
                var trackingLabel = await _ghtkService.GetTrackingOrderAsync(orderId);

                if (string.IsNullOrEmpty(trackingLabel))
                {
                    return NotFound($"Tracking label not found for Order ID: {orderId}");
                }

                return Ok(new { labelId = trackingLabel });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tracking order {orderId}: {ex.Message}");
                return StatusCode(500, "Internal server error while fetching tracking order.");
            }
        }
    }
}
