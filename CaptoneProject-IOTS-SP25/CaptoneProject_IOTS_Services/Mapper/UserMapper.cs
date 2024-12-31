using CaptoneProject_IOTS_BOs.DTO.RoleDTO;
using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.Models;
using CaptoneProject_IOTS_Service.Services.Implement;
using CaptoneProject_IOTS_Service.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class UserMapper
    {
        private static IMapService<User, UserDetailsResponseDTO> _userMapper = new MapService<User, UserDetailsResponseDTO>();
        private static IMapService<Role, RoleResponse> _roleMapper = new MapService<Role, RoleResponse>();

        public static UserDetailsResponseDTO mapToUserDetailResponse(User user)
        {
            UserDetailsResponseDTO data = _userMapper.MappingTo(user);
            List<Role> roleList = user.UserRoles.Select(ur => ur.Role).ToList();
            data.Roles = roleList?.Select(role => _roleMapper.MappingTo(role))?.ToList();

            return data;
        }
    }
}
