using CaptoneProject_IOTS_BOs.DTO.RabbitMQDTO;
using CaptoneProject_IOTS_Repository.Base;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaptoneProject_IOTS_API.Controllers.RabbitMQ
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMQController : ControllerBase
    {
        private readonly IRabbitMQService _rabbitMQService;

        public RabbitMQController(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost("send")]
        public IActionResult SendMessage([FromBody] RabbitMessageDTO request)
        {
            _rabbitMQService.SendChatMessage(request.Message);
            return Ok("Message sent to RabbitMQ");
        }
    }
}
