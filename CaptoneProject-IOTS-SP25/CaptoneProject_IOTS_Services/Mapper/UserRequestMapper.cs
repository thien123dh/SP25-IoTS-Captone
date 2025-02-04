using CaptoneProject_IOTS_BOs.DTO.UserDTO;
using CaptoneProject_IOTS_BOs.DTO.UserRequestDTO;
using CaptoneProject_IOTS_BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_Service.Mapper
{
    public static class UserRequestMapper
    {
        public static UserRequestResponseDTO  MappingToUserRequestResponseDTO(UserRequest userRequest)
        {
            return new UserRequestResponseDTO
            {
                Id = userRequest.Id,
                Email = userRequest.Email,
                UserRequestStatus = new UserRequestStatusDTO
                {
                    Id = userRequest.Status,
                    Label = userRequest?.StatusNavigation?.Label
                },
                CreatedDate = userRequest.CreatedDate,
                ActionDate = userRequest.ActionDate,
                Remark = userRequest.Remark,
                Role = userRequest.Role

            };
        }

        public static UserRequestDetailsResponseDTO MappingToUserRequestDetailsResponseDTO(UserRequest userRequest, UserDetailsResponseDTO userDetails)
        {
            return new UserRequestDetailsResponseDTO
            {
                userRequestInfo = MappingToUserRequestResponseDTO(userRequest),
                userDetails = userDetails

            };
        }
    }
}
