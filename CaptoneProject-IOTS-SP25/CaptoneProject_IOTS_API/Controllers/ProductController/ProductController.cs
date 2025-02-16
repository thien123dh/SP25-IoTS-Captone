using CaptoneProject_IOTS_BOs.Migrations;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.ProductController
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IotsDeviceRepository iotsDeviceRepository;

        public ProductController(
            IotsDeviceRepository iotsDeviceRepository
        )
        {
            this.iotsDeviceRepository = iotsDeviceRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(iotsDeviceRepository.GetAll());
        }
    }
}
