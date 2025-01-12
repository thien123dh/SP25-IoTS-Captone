using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.UserDTO
{
    public class UserRequestRequestDTO
    {
        public string Email { get; set; }
        public int UserRequestStatus { get; set; }
    }

    public class CreateUserRequestDTO
    {
        public string Email { get; set; }
    }
}
