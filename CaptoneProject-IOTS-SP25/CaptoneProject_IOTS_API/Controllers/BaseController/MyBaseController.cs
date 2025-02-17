using CaptoneProject_IOTS_BOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CaptoneProject_IOTS_API.Controllers.MyBaseController
{
    public class MyBaseController : ControllerBase
    {
        protected IActionResult GetActionResult(ResponseDTO response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(response);
            }
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(response);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(response);
            }

            return BadRequest(response);
        }
    }
}
