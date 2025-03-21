using CaptoneProject_IOTS_BOs.DTO.GHTKDTO;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        [HttpGet("{trackingId}")]
        public async Task<IActionResult> GetTrackingOrder(string trackingId)
        {
            try
            {
                var trackingLabel = await _ghtkService.GetTrackingOrderAsync(trackingId);

                if (trackingLabel == null)
                {
                    return NotFound($"Not found for Tracking Order : {trackingId}");
                }
                return Ok(trackingLabel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tracking order {trackingId}: {ex.Message}");
                return StatusCode(500, "Internal server error while fetching tracking order.");
            }
        }

        [HttpGet("print-label/{trackingId}")]
        public async Task<IActionResult> PrintLabel(string trackingId)
        {
            try
            {
                var pdfBytes = await _ghtkService.PrintLabelAsync(trackingId);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    return NotFound("Không tìm thấy nhãn vận đơn.");
                }

                return File(pdfBytes, "application/pdf", $"{trackingId}.pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching label {trackingId}: {ex.Message}");
                return StatusCode(500, "Internal server error while fetching label.");
            }
        }
    }
}
