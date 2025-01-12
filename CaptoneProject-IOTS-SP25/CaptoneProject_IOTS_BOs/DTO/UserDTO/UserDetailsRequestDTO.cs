using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptoneProject_IOTS_BOs.DTO.UserDTO
{
    public class UserDetailsRequestDTO
    {
        public string Email { get; set; }
        public string Fullname { set; get; }
        //public string Password { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int RoleId {get; set;}
    }

    public class UserRegisterDTO
    {
        [Required]
        public UserDetailsRequestDTO UserInfomation { get; set; }
        [Required]
        public string Otp { set; get; }
        [Required]
        public string Password { set; get; }
    }
}
