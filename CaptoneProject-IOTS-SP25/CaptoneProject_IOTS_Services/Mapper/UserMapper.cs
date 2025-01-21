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
        private static IMapService<User, UserResponseDTO> _userMapper = new MapService<User, UserResponseDTO>();
        private static IMapService<User, UserDetailsResponseDTO> _userDetailsMapper = new MapService<User, UserDetailsResponseDTO>();
        private static IMapService<Role, RoleResponse> _roleMapper = new MapService<Role, RoleResponse>();

        public static UserResponseDTO mapToUserResponse(User user)
        {
            if (user == null)
                return null;

            UserResponseDTO data = _userMapper.MappingTo(user);
            List<Role> roleList = user.UserRoles.Select(ur => ur.Role).ToList();
            data.Roles = roleList?.Select(role => _roleMapper.MappingTo(role))?.ToList();

            return data;
        }

        public static UserDetailsResponseDTO mapToUserDetailsResponse(User user)
        {
            if (user == null)
                return null;

            UserDetailsResponseDTO data = _userDetailsMapper.MappingTo(user);
            List<Role> roleList = user.UserRoles.Select(ur => ur.Role).ToList();
            data.Roles = roleList?.Select(role => _roleMapper.MappingTo(role))?.ToList();

            return data;
        }
    }
}
