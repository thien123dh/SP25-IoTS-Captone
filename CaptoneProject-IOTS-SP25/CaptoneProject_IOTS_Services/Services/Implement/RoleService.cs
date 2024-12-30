using CaptoneProject_IOTS_BOs;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Repository.Repository.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Services.Implement
{
    public class RoleService : IRoleService
    {
        private readonly RoleRepository _roleRepository;

        public RoleService(RoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<ResponseDTO> GetAllRoles()
        {
            IEnumerable<Role> roleList = _roleRepository.GetActiveRoles();

            if (roleList == null)
            {
                return new ResponseDTO
                {
                    IsSuccess = false,
                    Message = "Role is empty",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            return new ResponseDTO {
                IsSuccess = true,
                Message = "OK",
                StatusCode = HttpStatusCode.OK,
                Data = roleList.Select(
                        (item) => new
                        {
                            id = item.Id,
                            label = item.Label,
                            order = item.Orders,
                            isActive = true
                        }
                    )
                };
                
        }
    }
}
