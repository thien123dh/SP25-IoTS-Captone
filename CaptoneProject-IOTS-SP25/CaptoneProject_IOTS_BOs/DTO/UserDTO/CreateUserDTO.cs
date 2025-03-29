using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaptoneProject_IOTS_BOs.Constant.GenderConst;

namespace CaptoneProject_IOTS_BOs.DTO.UserDTO
{
    public class ContactInformationDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Phone { get; set; }
    }
    public class CreateUserDTO : ContactInformationDTO
    {
        [Required]
        public string? Fullname { set; get; }
        public string? Address { get; set; }
        public GenderEnum Gender { set; get; } = GenderEnum.Male;
        [Required]
        public int RoleId {get; set;}
    }

    public class UpdateUserDTO
    {
        [Required]
        public string Fullname { set; get; }

        public string? Address { set; get; }

        public string Phone { set; get; }
    }

    public class UpdateUserAvatarDTO
    {
        public string ImageUrl { set; get; }
    }
    public class UserRegisterDTO
    {
        [Required]
        public CreateUserDTO UserInfomation { get; set; }
        [Required]
        public string Otp { set; get; }
        [Required]
        public string Password { set; get; }
    }
}
