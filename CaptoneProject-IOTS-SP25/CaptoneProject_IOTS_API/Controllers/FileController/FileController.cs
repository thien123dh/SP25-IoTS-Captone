using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CaptoneProject_IOTS_BOs.DTO.FileDTO.FileDTO;

namespace CaptoneProject_IOTS_API.Controllers.FileController
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        IFileService fileService;
        public FileController (
            IFileService fileService
        )
        {
            this.fileService = fileService;
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
        [HttpPost("upload-files")]
        public async Task<IActionResult> UploadFile(
            IFormFile file
        )
        {
            return Ok(await fileService.UploadFile(file));
        }
    }
}
