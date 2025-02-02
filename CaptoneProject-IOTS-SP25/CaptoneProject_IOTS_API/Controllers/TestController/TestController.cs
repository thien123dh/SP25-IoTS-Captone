using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Attachment = CaptoneProject_IOTS_BOs.Models.Attachment;

namespace CaptoneProject_IOTS_API.Controllers.TestController
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AttachmentRepository attachmentRepository;
        public TestController(AttachmentRepository attachmentRepository)
        {
            this.attachmentRepository = attachmentRepository;
        }

        [HttpPost("create-or-update-attachment")]
        public IActionResult CreateOrUpdateAttachment([FromBody] Attachment payload)
        {
            attachmentRepository.Create(payload);
            return Ok();

        }
    }
}
