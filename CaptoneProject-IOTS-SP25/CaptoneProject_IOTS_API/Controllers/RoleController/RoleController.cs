using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CaptoneProject_IOTS_API.Controllers.RoleController
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            this._roleService = roleService;
        }
        // GET: api/<RoleController>
        [HttpGet]
        public Task<ResponseDTO> GetAllRoles()
        {
            return _roleService.GetAllRoles();
        }

    }
}
