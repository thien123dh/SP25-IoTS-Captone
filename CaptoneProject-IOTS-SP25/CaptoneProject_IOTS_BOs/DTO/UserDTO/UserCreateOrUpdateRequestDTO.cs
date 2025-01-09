using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.UserDTO
{
    public class UserCreateOrUpdateRequestDTO
    {
        public string Email { get; set; }
        public string Fullname { set; get; }
        public string Password { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int RoleId {get; set;}
    }
}
